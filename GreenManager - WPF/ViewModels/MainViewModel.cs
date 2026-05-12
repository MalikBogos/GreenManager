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
			Customer Bob = new Customer() { FirstName = "Bob", LastName = "Bogos", Email = "Bobbogos@gmail.com", PhoneNumber = "23844848023" };
			Customer Bob2 = new Customer() { FirstName = "Bobd2a", LastName = "Bogos", Email = "Bobbogos2a@gmail.com", PhoneNumber = "23844848023" };
			Customer Bob3 = new Customer() { FirstName = "Bobd3a", LastName = "Bogos", Email = "Bobbogos3a@gmail.com", PhoneNumber = "23844848023" };
			Customer Bob4 = new Customer() { FirstName = "Bob4a", LastName = "Bogos", Email = "Bobbogos4a@gmail.com", PhoneNumber = "23844848023" };
			Customer Bob5 = new Customer() { FirstName = "Bob5a", LastName = "Bogos", Email = "Bobbogos5a@gmail.com", PhoneNumber = "23844848023" };
			Customer Bob6 = new Customer() { FirstName = "Bob6a", LastName = "Bogos", Email = "Bobbogos6a@gmail.com", PhoneNumber = "23844848023" };

			using (var context = new GreenManagerDbContext())
			{
				context.Database.EnsureDeleted();
				context.Database.EnsureCreated();

				// Only add if the table is empty
				//if (!context.Customers.Any())
				{
				context.Customers.Add(Bob);
				context.Customers.Add(Bob2);
				context.Customers.Add(Bob3);
				context.Customers.Add(Bob4);
				context.Customers.Add(Bob5);
				context.Customers.Add(Bob6);
				context.SaveChanges();
				}

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