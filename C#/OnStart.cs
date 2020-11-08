using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
namespace BMBF_Manager
{
	public partial class App : Application
	{

		public void Application_StartupAsync(object sender, StartupEventArgs e)
		{
			MainWindow wnd = new MainWindow();
			wnd.Show();
			if (e.Args.Length == 1)
			{
				wnd.CustomProto(e.Args[0]);
			}
			
		}
	}
}