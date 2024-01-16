async function get_data(){
    let button = document.getElementById("auth-button");
    let response = await fetch("/",{
        method: "POST"
    });
    let res = await response.json();
    if ("status" in res){
        console.log(res.error);
    }
    else{
        
        const profile_button = `<li class="auth" id="auth-button"><a href="#">Hello ${res.name}</a></li>`;
        button.outerHTML =  profile_button;

    }
}
get_data();