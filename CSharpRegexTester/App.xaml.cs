using System;
using System.Windows;

using CSharpRegexTester.ViewModels;
using CSharpRegexTester.Views;

using Livet;

using Reactive.Bindings;

namespace CSharpRegexTester {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App {
		protected override void OnStartup(StartupEventArgs e) {
			DispatcherHelper.UIDispatcher = this.Dispatcher;
			UIDispatcherScheduler.Initialize();
			AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;
			var vm = new MainWindowViewModel();
			this.MainWindow = new MainWindow();
			this.MainWindow.DataContext = vm;
			this.MainWindow.ShowDialog();
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
			if (e.ExceptionObject is Exception ex) {
				Console.WriteLine("集約エラーハンドラ");
				Console.WriteLine(ex);
			} else {
				Console.WriteLine(e);
			}

			MessageBox.Show(
				"不明なエラーが発生しました。アプリケーションを終了します。",
				"エラー",
				MessageBoxButton.OK,
				MessageBoxImage.Error);

			Environment.Exit(1);
		}
	}
}
