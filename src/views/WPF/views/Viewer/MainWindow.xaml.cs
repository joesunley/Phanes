using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Phanes.Common;

namespace Phanes.View.Viewer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			Input.Start();

			Input.KeyDown += args =>
			{
				if (args.KeyCode == KeyCode.O &&
				    args.Modifiers.HasControlFlag())
				{
					OpenFileDialog dialog = new()
					{
						Filter = "XML Files (*.xml)|*.xml|PMAP Files (*.pmap)|*.pmap|All files (*.*)|*.*",
						AddExtension = true,
						CheckFileExists = true,
						CheckPathExists = true,
						Multiselect = false,
					};

					if (dialog.ShowDialog()!.Value)
						Dispatcher.Invoke(() => mapViewer.Load(File.ReadAllText(dialog.FileName)));
				}
			};
		}
	}
}