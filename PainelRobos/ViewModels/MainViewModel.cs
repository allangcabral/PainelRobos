using PainelRobos.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PainelRobos.ViewModels
{
    class MainViewModel: BaseViewModel
    {
		private const int _TEMPO_ESPERA_INICIAL = 10;
		
		private BackgroundWorker _backgroundInicio;
		private ManualResetEvent _backgroundOcupado;

		#region Propriedade com binding de controle de tela

		private string _local = $"Monitor Robôs - {System.Reflection.Assembly.GetAssembly(typeof(MainViewModel)).Location}";

		public string Local
		{
			get { return _local; }
			set { _local = value; OnPropertyChanged(); }

		}

		private bool _iniciando = true;

		public bool Iniciando
		{
			get { return _iniciando; }
			set { _iniciando = value; OnPropertyChanged(); }
		}


		private DateTime inicio = DateTime.Now;

		public DateTime Inicio
		{
			get { return inicio; }
			set { inicio = value; OnPropertyChanged(); }
		}

		private string _versao = $"{(Environment.Is64BitProcess ? "x64" : "x86")} | {Assembly.GetExecutingAssembly().GetName().Version}";

		public string Versao
		{
			get { return _versao; }
			set { _versao = value; OnPropertyChanged(); }
		}

		private string _mensagemInicio;

		public string MensagemInicio
		{
			get { return _mensagemInicio; }
			set { _mensagemInicio = value; OnPropertyChanged(); }
		}

		private bool _tempoParado;

		public bool TempoParado
		{
			get { return _tempoParado; }
			set 
			{ 
				_tempoParado = value; 
				OnPropertyChanged();
				if (TempoParado)
					_backgroundOcupado.Reset();
				else
					_backgroundOcupado.Set();
			}
		}


        #endregion

        #region Commands

        public Command IniciarCommand { get; }

		#endregion

		public MainViewModel()
		{
			IniciarCommand = new Command(ExecuteIniciarCommand);

			_backgroundInicio = new BackgroundWorker();
			_backgroundOcupado = new ManualResetEvent(false);
			_backgroundInicio.WorkerSupportsCancellation = true;
			_backgroundInicio.DoWork += _backgroundInicio_DoWork;
			_backgroundInicio.RunWorkerAsync();
			_backgroundOcupado.Set();

		}

		private void ExecuteIniciarCommand()
		{
			_backgroundInicio.CancelAsync();
			Iniciando = false;
		}

		private void _backgroundInicio_DoWork(object sender, DoWorkEventArgs e)
		{
			int segundos = 0;

			while (segundos < _TEMPO_ESPERA_INICIAL)
			{
				if (_backgroundInicio.CancellationPending == true)
					return;

				_backgroundOcupado.WaitOne();
				MensagemInicio = $"Começando em { _TEMPO_ESPERA_INICIAL - segundos } segundos";
				segundos++;

				Thread.Sleep(1000);
			}

			Iniciando = false;
		}
	}
}
