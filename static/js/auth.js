const btn_login = document.getElementById("btn-auth");
const span_error = document.getElementById("error");
btn_login.addEventListener("click", check_account);
async function check_account(event){
    event.preventDefault();
    let response = await fetch("/auth",{
        method: "POST",
        headers: {
            "Content-Type": "application/json",  
        },
        body: JSON.stringify({
            "Email": document.getElementById("email-login").value,
            "Password": document.getElementById("password-login").value,
        })
    });
    let res = await response.json(); 
    if (res.status == "OK"){
        window.document.location.href = "/";
    }
    else if (res.status == "Error"){
        span_error.style.display = "block";
        span_error.innerHTML  = res.error;
    }
}