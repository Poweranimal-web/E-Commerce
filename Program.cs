using System;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.FileProviders;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Net;
using MySqlX.XDevAPI;
public partial class  Customer{
    public int Id{ get; set; }
    public string Name{ get; set; }
    public string  Email { get; set; }
    public string  Password { get; set; }

}
public partial class  CustomerLogin{
    public string  Email { get; set; }
    public string  Password { get; set; }

}
public class Code{
    public string code {get;set;}
}
class Response{
    public string Status { get; set; }
    public string Error { get; set; }
    public Response(string name, string error_name="no any errors"){
        Status = name;
        Error = error_name;
    }    
}
public class Requests{
    public string Status {get;set;}
    public string? Data {get;set;}
    public Dictionary<string, string>? ExtraData {get;set;}
}

public class categories{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<goods> goodss { get;set;}
}
public class goods{
    public int Id { get; set; }
    public string goods_name { get; set; }
    public int goods_price { get; set; }
    public int goods_amount { get; set; }
    public string goods_description { get; set; }
    public int сategoriesId {get;set;}
    public categories categories {get;set;}
    public ICollection<basket> baskets { get;set;}
}
public class goodsdata <Type>{
    public List<Type> Data{get;set;}
    public int Length{get;set;}
    public goodsdata(List<Type> products, int length){
        Data = products;
        Length = length;
    }
}
public class basket{
    public int Id{get;set;}
    public string cookie_name{get;set;}
    public int goodsId{get;set;}
    public int amount{get; set;}
    public goods goods{get;set;} 
}
public class LieInBasket{
    public int Id_product { get; set; }
    public string basket_name{get;set;}
    public string goods_name { get; set; }
    public int goods_id { get; set; }
    public int goods_price { get; set; }
    public int amount{get; set;}    
}
public class DetailBasketData{
    public string cookie;
    public int id;
}

class Email {
    public string To;
    public string Name;
    char[] symbols = new char[] {'a','b','c','d','e','f','g','h','i','j','k','l',
    'm','n','o','p','q','r','s','t','u','v','w','x','y','z'};
    char[] numbers = new char[] {'1','2','3','4','5','6','7','8','9'};
    char[] code = new char[6];
    string generated_code;
    public string Code {
        get{
            return generated_code;
        }
        set{
            generated_code = value;
        }
    }
    public Email(string to, string name){
        To = to;
        Name = name;
    }
    void GenerateCode(){
        for(int i=0; i<code.Length;i++){
            Random num = new Random();
            int letter_or_number = num.Next(1,3);
            if (letter_or_number==1){
                int random_number_letter = num.Next(0,this.symbols.Length);
                code[i] = symbols[random_number_letter];
            }
            else {
                int random_number = num.Next(0,this.numbers.Length);
                code[i] = numbers[random_number];
            }

        }
        Code = new string(code);
    }
    
     public string SendMail()
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("buyahh11@gmail.com", "myuakwbqkunxppgt");
            // Specify the email sender.
            // Create a mailing address that includes a UTF8 character
            // in the display name.
            MailAddress from = new MailAddress("buyahh11@gmail.com");
            // Set destinations for the email message.
            MailAddress to = new MailAddress(this.To);
            // Specify the message content.
            MailMessage message = new MailMessage(from, to);
            GenerateCode();
            message.Body = $"Hello {this.Name}, here is your code: {this.Code} ";
            message.BodyEncoding =  System.Text.Encoding.UTF8;
            message.Subject = "Test email";
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            // The userState can be any object that allows your callback
            // method to identify this send operation.
            // For this example, the userToken is a string constant.
            string userState = "test message1";
            client.SendAsync(message, userState);
            // If the user canceled the send, and mail hasn't been sent yet,
            // then cancel the pending operation.
            // Clean up.
            return Code;

        }


}
public partial class EContext: DbContext{
    public virtual DbSet<Customer> Customers {get;set;}
    public virtual DbSet<categories> categories {get;set;}
    public virtual DbSet<goods> goods {get;set;}
    public virtual DbSet<basket> baskets {get;set;}
     
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    
        string connectionString = "server=localhost;user=root;password=root123;database=serviceappliances";
        optionsBuilder.UseMySQL(connectionString);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<goods>()
        .HasMany(e => e.baskets)
        .WithOne(e =>e.goods)
        .HasForeignKey(e => e.goodsId)
        .HasPrincipalKey(e => e.Id);
        modelBuilder.Entity<categories>()
        .HasMany(e => e.goodss)
        .WithOne(e => e.categories)
        .HasForeignKey(e => e.сategoriesId)
        .HasPrincipalKey(e => e.Id);
        
    }

}
namespace Nikita{
    static class Programm{
        static void Main(){
                var builder = WebApplication.CreateBuilder();
                builder.Services.AddDistributedMemoryCache();
                builder.Services.AddSession();
                builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler =  ReferenceHandler.IgnoreCycles);
                var app = builder.Build();
                EContext db = new EContext();
                app.UseSession();
                app.UseStaticFiles(new StaticFileOptions{
                    FileProvider = new PhysicalFileProvider(
                        Path.Combine(builder.Environment.ContentRootPath, "static")),
                        RequestPath = "/Static"
                });

                app.Map("/", async(context) =>{
                    HttpRequest request = context.Request;
                    switch(request.Method){
                        case "GET":
                            await context.Response.SendFileAsync("html/main.html"); 
                            break;
                        case "POST":
                            if (request.ContentType == "application/json"){
                                var MainRequest = await request.ReadFromJsonAsync<Requests>();
                                if (MainRequest.Status == "get_profile_data"){
                                    if(context.Session.Keys.Contains("email")){
                                        string email = context.Session.GetString("email");
                                        var user_data = await db.Customers.Where(p => p.Email==email).FirstAsync();
                                        await context.Response.WriteAsJsonAsync<Customer>(user_data);   
                                    }
                                    else{
                                        Response response = new Response("Error", "Your session expired or didn't exist");
                                        await context.Response.WriteAsJsonAsync<Response>(response);   
                                    }
                                }
                                else if(MainRequest.Status == "get_goods_data"){
                                    var goods_data = db.goods.ToList();
                                    goodsdata<goods> Goods = new goodsdata<goods>(goods_data,goods_data.Count());
                                    await context.Response.WriteAsJsonAsync<goodsdata<goods>>(Goods);  
                                }
                                else if(MainRequest.Status == "search"){
                                    if (MainRequest.Data.Length > 0){
                                        var name = $"%{MainRequest.Data}%";
                                        var result = db.goods.FromSqlRaw("SELECT * FROM goods WHERE goods_name LIKE {0}", name).ToList();
                                        goodsdata<goods> Goods = new goodsdata<goods>(result,result.Count());
                                        await context.Response.WriteAsJsonAsync<goodsdata<goods>>(Goods); 
                                    }
                                    else{
                                        var result = db.goods.ToList();
                                        goodsdata<goods> Goods = new goodsdata<goods>(result,result.Count());
                                        await context.Response.WriteAsJsonAsync<goodsdata<goods>>(Goods); 
                                    }
                                    
                                }   
                                
                            }
                            break;
                                
                    }
                });
                app.Map("/code", async(context)=>{
                    HttpRequest request = context.Request;
                    Email email = new Email(context.Session.GetString("email"), context.Session.GetString("name")); 
                    if (context.Session.Keys.Contains("Check_email") && context.Session.GetString("Check_email")=="false"){
                        switch(request.Method){
                            case "GET":
                                string code = await Task.Run(email.SendMail);
                                context.Session.SetString("code", code);
                                await context.Response.SendFileAsync("html/code.html");
                                break;
                            case "POST":
                                Code Code = await request.ReadFromJsonAsync<Code>();
                                if (Code.code==context.Session.GetString("code")){
                                    var session_data = context.Session.GetString("customer_data");
                                    Customer customer_data = JsonSerializer.Deserialize<Customer>(session_data);
                                    await db.Customers.AddAsync(customer_data);
                                    await db.SaveChangesAsync();
                                    context.Session.SetString("Check_email", "true");
                                    Response response = new Response("OK");
                                    await context.Response.WriteAsJsonAsync<Response>(response);
                                }
                                else{
                                    Response response = new Response("Error", "Code doesn't match");
                                    await context.Response.WriteAsJsonAsync<Response>(response); 
                                }
                                break;

                        }
                    }
                    else{
                        context.Response.Redirect("/");
                    }
                });
                app.Map("/auth", async(context)=>{
                    HttpRequest request = context.Request;
                    switch(request.Method){
                        case "GET":
                            await context.Response.SendFileAsync("html/auth.html");
                            break;
                        case "POST":
                                var customer_data = await request.ReadFromJsonAsync<CustomerLogin>();
                                bool exist_customer =  await db.Customers.AnyAsync(p => p.Email==customer_data.Email && p.Password==customer_data.Password);
                                if (!exist_customer){
                                    Response response = new Response("Error", "Account doesn't exist");
                                    await context.Response.WriteAsJsonAsync<Response>(response);    
                                }
                                else{
                                    Response response = new Response("OK");
                                    context.Session.SetString("email", customer_data.Email);
                                    await context.Response.WriteAsJsonAsync<Response>(response);   
                                }  
                                break;
                    }
                });
                app.Map("/reg", async(context)=>{
                    HttpRequest request = context.Request;
                    switch(request.Method){
                        case "GET":
                            await context.Response.SendFileAsync("html/reg.html");
                            break;
                        case "POST":
                                var customer_data = await request.ReadFromJsonAsync<Customer>();
                                string jsonString = JsonSerializer.Serialize(customer_data);
                                bool exist_customer =  await db.Customers.AnyAsync(p => p.Email==customer_data.Email);
                                if (!exist_customer){
                                    context.Session.SetString("customer_data", jsonString);
                                    context.Session.SetString("Check_email", "false");
                                    context.Session.SetString("email", customer_data.Email);
                                    context.Session.SetString("name", customer_data.Name);
                                    Response response = new Response("Error", "Check_email");
                                    await context.Response.WriteAsJsonAsync<Response>(response);    
                                }
                                else{
                                    Response response = new Response("Error", "Account has already existed");
                                    await context.Response.WriteAsJsonAsync<Response>(response);   
                                }  
                                break;
                    }
                });
                app.Map("/basket", async(context)=>{
                    HttpRequest request = context.Request;
                    switch(request.Method){
                        case "GET":
                            await context.Response.SendFileAsync("html/basket.html");
                            break;
                        case "POST":
                            Requests jsondata = await request.ReadFromJsonAsync<Requests>();
                            if (jsondata.Status == "get_basket"){
                                // var list_products = db.baskets.Where(e => e.cookie_name==jsondata.Data).ToList();
                                var list_products = (from b in db.baskets
                                                    join g in db.goods on b.goodsId equals g.Id 
                                                    where b.cookie_name == jsondata.Data
                                                    select new LieInBasket{Id_product=g.Id,basket_name=b.cookie_name, 
                                                                        goods_id=g.Id, goods_name=g.goods_name, goods_price=g.goods_price, amount=b.amount}).ToList();
        
                                goodsdata<LieInBasket> Goods = new goodsdata<LieInBasket>(list_products,list_products.Count());
                                await context.Response.WriteAsJsonAsync<goodsdata<LieInBasket>>(Goods);
                            }
                            else if (jsondata.Status == "add_item"){

                                await db.baskets.Where(b => b.cookie_name == jsondata.ExtraData["cookie"] && b.goodsId == Convert.ToInt32(jsondata.ExtraData["id"]))
                                .ExecuteUpdateAsync(s=>s.SetProperty(u=>u.amount, u=>u.amount+1));
                                await db.SaveChangesAsync();
                                Response res = new Response("Updated");
                                await context.Response.WriteAsJsonAsync<Response>(res);
                            }
                            else if (jsondata.Status == "remove_item"){

                                await db.baskets.Where(b => b.cookie_name == jsondata.ExtraData["cookie"] && b.goodsId == Convert.ToInt32(jsondata.ExtraData["id"]))
                                .ExecuteUpdateAsync(s=>s.SetProperty(u=>u.amount, u=>u.amount-1));
                                await db.SaveChangesAsync();
                                Response res = new Response("Updated");
                                await context.Response.WriteAsJsonAsync<Response>(res);
                            }
                            else if(jsondata.Status == "remove_product"){
                                await db.baskets.Where(b => b.cookie_name == jsondata.ExtraData["cookie"] && b.goodsId == Convert.ToInt32(jsondata.ExtraData["id"]))
                                .ExecuteDeleteAsync();
                                await db.SaveChangesAsync();                          
                                Response res = new Response("Deleted");
                                await context.Response.WriteAsJsonAsync<Response>(res);
                            }

                            break;
                    }
                });
                app.Run();
        } 
    }
}
