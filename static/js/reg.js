const btn_reg = document.getElementById("btn-reg");
const span_error = document.getElementById("error");
btn_reg.addEventListener("click", create_account);
function validation(email){
    let dog = "@";
    return email.includes(dog);
}
function compare_password(password,repeat_password){
    if (password == repeat_password){
        return true;
    }
    else{
        return false;
    }

}
async function create_account(event){
    event.preventDefault();
    const check_email = validation(document.getElementById("email-reg").value);
    const check_match_password = compare_password(
        document.getElementById("password-reg").value, document.getElementById("repeatpassword-reg").value);
    if (check_email){
        if (check_match_password){
            let response = await fetch("/reg",{
                method: "POST",
                headers: {
                    "Content-Type": "application/json",  
                },
                body: JSON.stringify({
                    "Name": document.getElementById("name-reg").value,
                    "Email": document.getElementById("email-reg").value,
                    "Password": document.getElementById("password-reg").value,
                })
            });
            let res = await response.json(); 
            if (res.status == "OK"){
                window.document.location.href = "/";
            }
            else if (res.status == "Error"){
                    if (res.error == "Check_email"){
                        window.document.location.href = "/code";
                    }
                    else{
                        span_error.style.display = "block";
                        span_error.innerHTML  = res.error;
                    }
            }
        }
        else{
            span_error.style.display = "block";
            span_error.innerHTML  = "Passwords don't match";
        }
    }
    else{
        span_error.style.display = "block";
        span_error.innerHTML  = "Invalid format of email";
    }
}