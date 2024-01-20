const list_cards = document.getElementById("list-card"); 
const search_input = document.getElementById("search");
search_input.addEventListener("input", search);
async function get_profile_data(){
    let button = document.getElementById("auth-button");

    let response = await fetch("/",{
        method: "POST",
        headers: {
            "Content-Type": "application/json",  
        },
        body: JSON.stringify({
            "Status": "get_profile_data",
            "Data":  ""
        })
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
async function get_goods_data(){
    let response = await fetch("/",{
        method: "POST",
        headers: {
            "Content-Type": "application/json",  
        },
        body: JSON.stringify({
            "Status": "get_goods_data",
            "Data":  ""

        })
    });
    let res = await response.json();
    if (res.length > 0){
        res.data.map((product)=>{
            list_cards.innerHTML += `<div class="card">
            <img src="/Static/img/WIN_20220925_17_36_16_Pro.jpg" alt="Avatar" style="width: 100%;">
            <div class="container">
                <h4><b>${product.goods_name}</b></h4>
                <p>${product.goods_price} UAH</p>
                <button class="btn-show">Show</button>
            </div>
        </div>`
        })
    }
}
async function search(event){
    let value = event.target.value;
    let response = await fetch("/",{
        method: "POST",
        headers: {
            "Content-Type": "application/json",  
        },
        body: JSON.stringify({
            "Status": "search",
            "Data":  value
        })
    });
    let res = await response.json();
    if (res.length > 0){
        list_cards.innerHTML = "";
        res.data.map((product)=>{
            list_cards.innerHTML += `<div class="card">
            <img src="/Static/img/WIN_20220925_17_36_16_Pro.jpg" alt="Avatar" style="width: 100%;">
            <div class="container">
                <h4><b>${product.goods_name}</b></h4>
                <p>${product.goods_price} UAH</p>
                <button class="btn-show">Show</button>
            </div>
        </div>`
        })
    }
    else{
        list_cards.innerHTML = "";
        list_cards.innerHTML += `<h2>Not found</h2>`;
    }
}
get_goods_data();
get_profile_data();