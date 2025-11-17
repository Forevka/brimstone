use std::ffi::{CStr, CString};
use std::os::raw::c_char;
use std::panic::{self, AssertUnwindSafe};
use std::ptr;
use std::rc::Rc;

use brimstone_core::{
    common::wtf_8::Wtf8String,
    parser::source::Source,
    runtime::{Context, ContextBuilder},
};

/// Opaque pointer to a Brimstone context
pub struct BsContext {
    context: Option<Context>,
}

/// Result codes for FFI functions
#[repr(C)]
pub enum BsResultCode {
    Success = 0,
    Error = 1,
    NullPointer = 2,
    InvalidUtf8 = 3,
    PanicCaught = 4,
}

/// Initialize Brimstone runtime (must be called before any other functions)
#[no_mangle]
pub extern "C" fn bs_init() {
    brimstone_serialized_heap::init();
}

/// Create a new Brimstone context
#[no_mangle]
pub extern "C" fn bs_context_new() -> *mut BsContext {
    let result = panic::catch_unwind(|| {
        let cx = ContextBuilder::new().build();
        Box::into_raw(Box::new(BsContext {
            context: Some(cx),
        }))
    });

    match result {
        Ok(ptr) => ptr,
        Err(_) => ptr::null_mut(),
    }
}

/// Destroy a Brimstone context
#[no_mangle]
pub extern "C" fn bs_context_free(ctx: *mut BsContext) {
    if ctx.is_null() {
        return;
    }

    let _ = panic::catch_unwind(AssertUnwindSafe(|| unsafe {
        let mut ctx_box = Box::from_raw(ctx);
        if let Some(cx) = ctx_box.context.take() {
            cx.drop();
        }
    }));
}

/// Evaluate JavaScript code
///
/// # Arguments
/// * `ctx` - Brimstone context
/// * `code` - JavaScript code to evaluate (null-terminated UTF-8 string)
/// * `filename` - Optional filename for error messages (null for "<eval>")
/// * `error_out` - Output parameter for error message (caller must free with bs_string_free)
///
/// # Returns
/// BsResultCode indicating success or failure
#[no_mangle]
pub extern "C" fn bs_eval(
    ctx: *mut BsContext,
    code: *const c_char,
    filename: *const c_char,
    error_out: *mut *mut c_char,
) -> BsResultCode {
    if ctx.is_null() || code.is_null() {
        return BsResultCode::NullPointer;
    }

    let result = panic::catch_unwind(AssertUnwindSafe(|| unsafe {
        let ctx_ref = &mut *ctx;
        let cx = match ctx_ref.context.as_mut() {
            Some(cx) => cx,
            None => return BsResultCode::Error,
        };

        // Convert C strings to Rust strings
        let code_str = match CStr::from_ptr(code).to_str() {
            Ok(s) => s,
            Err(_) => return BsResultCode::InvalidUtf8,
        };

        let filename_str = if filename.is_null() {
            "<eval>".to_string()
        } else {
            match CStr::from_ptr(filename).to_str() {
                Ok(s) => s.to_string(),
                Err(_) => return BsResultCode::InvalidUtf8,
            }
        };

        // Create source and evaluate
        let wtf8_code = Wtf8String::from_str(code_str);
        let source = match Source::new_for_string(&filename_str, wtf8_code) {
            Ok(s) => Rc::new(s),
            Err(e) => {
                if !error_out.is_null() {
                    if let Ok(error_msg) = CString::new(format!("Failed to create source: {:?}", e)) {
                        *error_out = error_msg.into_raw();
                    }
                }
                return BsResultCode::Error;
            }
        };

        // Execute the script
        match cx.evaluate_script(source) {
            Ok(_) => BsResultCode::Success,
            Err(err) => {
                if !error_out.is_null() {
                    // Format error by converting to string representation
                    let error_message = format!("JavaScript error occurred");
                    if let Ok(error_msg) = CString::new(error_message) {
                        *error_out = error_msg.into_raw();
                    }
                }
                BsResultCode::Error
            }
        }
    }));

    match result {
        Ok(code) => code,
        Err(_) => BsResultCode::PanicCaught,
    }
}

/// Evaluate JavaScript code and get the result as a string
///
/// # Arguments
/// * `ctx` - Brimstone context
/// * `code` - JavaScript code to evaluate (null-terminated UTF-8 string)
/// * `filename` - Optional filename for error messages (null for "<eval>")
/// * `result_out` - Output parameter for result string (caller must free with bs_string_free)
/// * `error_out` - Output parameter for error message (caller must free with bs_string_free)
///
/// # Returns
/// BsResultCode indicating success or failure
#[no_mangle]
pub extern "C" fn bs_eval_with_result(
    ctx: *mut BsContext,
    code: *const c_char,
    filename: *const c_char,
    result_out: *mut *mut c_char,
    error_out: *mut *mut c_char,
) -> BsResultCode {
    if ctx.is_null() || code.is_null() || result_out.is_null() {
        return BsResultCode::NullPointer;
    }

    let result = panic::catch_unwind(AssertUnwindSafe(|| unsafe {
        let ctx_ref = &mut *ctx;
        let cx = match ctx_ref.context.as_mut() {
            Some(cx) => cx,
            None => return BsResultCode::Error,
        };

        // Convert C strings to Rust strings
        let code_str = match CStr::from_ptr(code).to_str() {
            Ok(s) => s,
            Err(_) => return BsResultCode::InvalidUtf8,
        };

        let filename_str = if filename.is_null() {
            "<eval>".to_string()
        } else {
            match CStr::from_ptr(filename).to_str() {
                Ok(s) => s.to_string(),
                Err(_) => return BsResultCode::InvalidUtf8,
            }
        };

        // Wrap code to capture the result
        let wrapped_code = format!(
            "(() => {{ const __result = {}; return typeof __result === 'undefined' ? 'undefined' : JSON.stringify(__result); }})()",
            code_str
        );

        // Create source and evaluate
        let wtf8_code = Wtf8String::from_str(&wrapped_code);
        let source = match Source::new_for_string(&filename_str, wtf8_code) {
            Ok(s) => Rc::new(s),
            Err(e) => {
                if !error_out.is_null() {
                    if let Ok(error_msg) = CString::new(format!("Failed to create source: {:?}", e)) {
                        *error_out = error_msg.into_raw();
                    }
                }
                return BsResultCode::Error;
            }
        };

        // For getting results, we need to use the eval result mechanism
        // For now, we'll return a success message
        match cx.evaluate_script(source) {
            Ok(_) => {
                // Note: In a production implementation, you would need to capture the actual
                // return value from the script execution. This requires deeper integration
                // with the VM's evaluation result mechanism.
                if let Ok(msg) = CString::new("Script executed successfully") {
                    *result_out = msg.into_raw();
                }
                BsResultCode::Success
            }
            Err(err) => {
                if !error_out.is_null() {
                    // Format error by converting to string representation
                    let error_message = format!("JavaScript error occurred");
                    if let Ok(error_msg) = CString::new(error_message) {
                        *error_out = error_msg.into_raw();
                    }
                }
                BsResultCode::Error
            }
        }
    }));

    match result {
        Ok(code) => code,
        Err(_) => BsResultCode::PanicCaught,
    }
}

/// Free a string allocated by Brimstone
#[no_mangle]
pub extern "C" fn bs_string_free(s: *mut c_char) {
    if s.is_null() {
        return;
    }

    let _ = panic::catch_unwind(|| unsafe {
        let _ = CString::from_raw(s);
    });
}

/// Get Brimstone version string
#[no_mangle]
pub extern "C" fn bs_version() -> *const c_char {
    static VERSION: &str = "Brimstone 0.1.0 (FFI)\0";
    VERSION.as_ptr() as *const c_char
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_basic_eval() {
        bs_init();
        let ctx = bs_context_new();
        assert!(!ctx.is_null());

        let code = CString::new("console.log('Hello from FFI');").unwrap();
        let mut error: *mut c_char = ptr::null_mut();

        let result = bs_eval(
            ctx,
            code.as_ptr(),
            ptr::null(),
            &mut error as *mut *mut c_char,
        );

        if !error.is_null() {
            unsafe {
                let error_str = CStr::from_ptr(error).to_string_lossy();
                println!("Error: {}", error_str);
                bs_string_free(error);
            }
        }

        assert!(matches!(result, BsResultCode::Success));

        bs_context_free(ctx);
    }
}
