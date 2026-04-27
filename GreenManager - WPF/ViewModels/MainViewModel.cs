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

			using (var context = new GreenManagerDbContext())
			{
				context.Add(malik);
				context.SaveChanges();
			}

		}
	}

}
