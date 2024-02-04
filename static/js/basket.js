const list_products = document.getElementById("change-basket");
const buttons_add_item = document.getElementsByClassName("add-amount-button");
const buttons_remove_item = document.getElementsByClassName("remove-amount-button");
const buttons_remove_product = document.getElementsByClassName("delete-element-button");
list_products.nextSibling
async function add_item(event){
    event.preventDefault();
    let response = await fetch("/basket",{
        method: "POST",
        headers: {
            "Content-Type": "application/json", 
        },
        body : JSON.stringify({
            "Status":"add_item",
            "ExtraData": {"cookie": getCookie("basket-name"), "id":event.target.value},
        }),
    });
    let res = await response.json();
    console.log(res);
    if (res.status == "Updated"){
        event.target.nextElementSibling.innerText = Number(event.target.nextElementSibling.innerText) + 1;
        event.target.closest(".amount-block").previousElementSibling.children[0].innerHTML = 
        Number(event.target.closest(".amount-block").previousElementSibling.children[0].innerHTML) +  Number(event.target.closest(".amount-block").previousElementSibling.children[1].value);
        document.getElementById("price").innerHTML =  Number(document.getElementById("price").innerHTML) + Number(event.target.closest(".amount-block").previousElementSibling.children[1].value);
        document.getElementById("total").innerHTML =  Number(document.getElementById("total").innerHTML) + Number(event.target.closest(".amount-block").previousElementSibling.children[1].value);
    }
}
async function remove_item(event){
    event.preventDefault();
    let response = await fetch("/basket",{
        method: "POST",
        headers: {
            "Content-Type": "application/json", 
        },
        body : JSON.stringify({
            "Status":"remove_item",
            "ExtraData": {"cookie": getCookie("basket-name"), "id":event.target.value},
        }),
    });
    let res = await response.json();
    if (res.status == "Updated"){
        event.target.previousElementSibling.innerText = Number(event.target.previousElementSibling.innerText) - 1;
        event.target.closest(".amount-block").previousElementSibling.children[0].innerHTML = 
        Number(event.target.closest(".amount-block").previousElementSibling.children[0].innerHTML) -  Number(event.target.closest(".amount-block").previousElementSibling.children[1].value);
        document.getElementById("price").innerHTML =  Number(document.getElementById("price").innerHTML) - Number(event.target.closest(".amount-block").previousElementSibling.children[1].value);
        document.getElementById("total").innerHTML =  Number(document.getElementById("total").innerHTML) - Number(event.target.closest(".amount-block").previousElementSibling.children[1].value);
    }
}
async function remove_product(event){
    event.preventDefault();
    let response = await fetch("/basket",{
        method: "POST",
        headers: {
            "Content-Type": "application/json", 
        },
        body : JSON.stringify({
            "Status":"remove_product",
            "ExtraData": {"cookie": getCookie("basket-name"), "id":event.target.value},
        }),
    });
    let res = await response.json();
    if (res.Status == "Deleted"){
        
    }

}
function getCookie(name) {
    let matches = document.cookie.match(new RegExp(
      "(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
    ));
    return matches ? decodeURIComponent(matches[1]) : undefined;
  }
async function get_basket(){
    let response = await fetch("/basket",{
        method: "POST",
        headers: {
            "Content-Type": "application/json", 
        },
        body : JSON.stringify({
            "Status":"get_basket",
            "Data": getCookie("basket-name"),
        }),
    })
    let res = await response.json();
    let price = 0;
    if (res.length > 0){
        
        res.data.map((product)=>{
            price += product.goods_price* product.amount;
            list_products.innerHTML += `
            <div class="card">
                    <img src="/Static/img/WIN_20220925_17_36_16_Pro.jpg" alt="image_goods" style="width: 250px; border-bottom-left-radius: 10px;border-top-left-radius: 10px;"/>
                    <div class="cl2">
                        <h2 style="margin-top: 0px;">${product.goods_name}</h2>
                        <button class="delete-element-button" style="margin-bottom: 10px;" value="${product.goods_id}">Delete</button>
                    </div>
                    <div class="cl3">
                        <div class="price-element">
                            <p class="price-text">${product.goods_price* product.amount}</p>
                            <input type="hidden" value=${product.goods_price}>
                        </div>
                        <div class="amount-block" style="margin-bottom:14px;display: flex; flex-direction:row;"><button class="add-amount-button" value="${product.goods_id}">+</button><p class="amount" id="amount">${product.amount}</p><button class="remove-amount-button" value="${product.goods_id}">-</button></div>
                    </div>
            </div>`
        
    }
        )
        document.getElementById("price").innerHTML = price;
        document.getElementById("total").innerHTML = price;
        for(var i=0;i<buttons_add_item.length;i++){
            buttons_add_item[i].addEventListener("click", add_item);
        }
        for(var i=0;i<buttons_remove_item.length;i++){
            buttons_remove_item[i].addEventListener("click", remove_item);
        }
        for(var i=0;i<buttons_remove_product.length;i++){
            buttons_remove_product[i].addEventListener("click", remove_product);
        }
    }
}
get_basket();