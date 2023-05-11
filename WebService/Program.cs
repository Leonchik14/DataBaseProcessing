/*
 * Крайнов Леонид Игоревич БПИ223
*/
using NewVariant.Exceptions;
using NewVariant.Interfaces;
using NewVariant.Models;
using DataBaseLib;
namespace WebService
{
    public class Program
    {
        public static DataBase dataBase = new DataBase();
        public static DataAccessLayer dataAccessLayer = new DataAccessLayer();
        public static IEnumerable<object> dataList = new List<Object>();
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}