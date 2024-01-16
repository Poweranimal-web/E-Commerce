using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Extensions.FileProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;


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
// class Customer{
//     public string Name{ get; set; }
//     public string  Email { get; set; }
//     public string  Password { get; set; }
// }
class Response{
    public string Status { get; set; }
    public string Error { get; set; }
    public Response(string name, string error_name="no any errors"){
        Status = name;
        Error = error_name;
    }    

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
     
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = "server=localhost;user=root;password=root123;database=serviceappliances";
        optionsBuilder.UseMySQL(connectionString);
    }
}
namespace Nikita{
    static class Programm{
        static void Main(){
                var builder = WebApplication.CreateBuilder();
                builder.Services.AddDistributedMemoryCache();
                builder.Services.AddSession();
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
                            if(context.Session.Keys.Contains("email")){
                                string email = context.Session.GetString("email");
                                var user_data = await db.Customers.Where(p => p.Email==email).FirstAsync();
                                await context.Response.WriteAsJsonAsync<Customer>(user_data);   
                            }
                            else{
                                Response response = new Response("Error", "Your session expired or didn't exist");
                                await context.Response.WriteAsJsonAsync<Response>(response);   
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
                                    // await db.Customers.AddAsync(customer_data);
                                    // await db.SaveChangesAsync();
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
                app.Run();
        } 
    }
}
