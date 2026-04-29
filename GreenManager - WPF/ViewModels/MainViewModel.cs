using Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Models;
using Models.Data;

namespace GreenManager___WPF.ViewModels
{

	public class MainViewModel
	{
		public MainViewModel()
		{
			Customer malik = new Customer() { FirstName = "Malik", LastName = "Bogos", Email = "malikbogos@gmail.com", PhoneNumber = "23844848023" };
			Customer malik2 = new Customer() { FirstName = "Malikd2a", LastName = "Bogos", Email = "malikbogos2a@gmail.com", PhoneNumber = "23844848023" };
			Customer malik3 = new Customer() { FirstName = "Malikd3a", LastName = "Bogos", Email = "malikbogos3a@gmail.com", PhoneNumber = "23844848023" };
			Customer malik4 = new Customer() { FirstName = "Malik4a", LastName = "Bogos", Email = "malikbogos4a@gmail.com", PhoneNumber = "23844848023" };
			Customer malik5 = new Customer() { FirstName = "Malik5a", LastName = "Bogos", Email = "malikbogos5a@gmail.com", PhoneNumber = "23844848023" };
			Customer malik6 = new Customer() { FirstName = "Malik6a", LastName = "Bogos", Email = "malikbogos6a@gmail.com", PhoneNumber = "23844848023" };

			using (var context = new GreenManagerDbContext())
			{
				//context.Database.EnsureDeleted();
				//context.Database.EnsureCreated();

				// Only add if the table is empty
				//if (!context.Customers.Any())
				//{
					context.Customers.Add(malik);
					context.Customers.Add(malik2);
					context.Customers.Add(malik3);
					context.Customers.Add(malik4);
					context.Customers.Add(malik5);
					context.Customers.Add(malik6);
					context.SaveChanges();
				//}

				//var voorbeeld1 = context.Customers.ToList();

				//foreach(Customer c in voorbeeld1)
				//{
				//	Console.WriteLine($"Id is: {c.Id}");
				//	Console.WriteLine($"First name is: {c.FirstName}");
				//	Console.WriteLine($"Last name is: {c.LastName}");
				//	Console.WriteLine($"Email is: {c.Email}");
				//}
			}

			
		}
	}
}