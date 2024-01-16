const submit_button = document.getElementById("btn-auth");
const span_error = document.getElementById("error");
submit_button.addEventListener("click", check_code);
async function check_code(event) {
    event.preventDefault(); 
    let response = await fetch("/code", 
    {
    method: "POST",
    headers: {
        "Content-Type": "application/json",  
    },
    body: JSON.stringify({
        "Code": document.getElementById("code").value,
    })}
    );
    let res = await response.json(); 
    if (res.status == "OK"){
        window.document.location.href = "/";
    }
    else if (res.status == "Error"){
        span_error.style.display = "block";
        span_error.innerHTML = res.error;
    }
}
