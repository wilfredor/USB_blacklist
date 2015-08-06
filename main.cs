using System;
using System.Linq;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Management;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Security;
namespace USBService {
	public class WMIEvent: System.ServiceProcess.ServiceBase {
		private System.ComponentModel.Container components;

		public WMIEvent() {
			InitializeComponent();
		}
		static void Main() {

			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] {
				new WMIEvent()
			};
			ServiceBase.Run(ServicesToRun);

		}
		private void InitializeComponent() {
			components = new System.ComponentModel.Container();
			this.ServiceName = "WMIEvent";
		}
		protected override void OnStart(string[] args) {
			WMIEvent we = new WMIEvent();
			ManagementEventWatcher w = null;
			WqlEventQuery q;
			ManagementOperationObserver observer = new
			ManagementOperationObserver();
			// Bind to local machine
			ManagementScope scope = new ManagementScope("root\\CIMV2");
			scope.Options.EnablePrivileges = true; //sets required

			try {
				q = new WqlEventQuery();
				q.EventClassName = "__InstanceOperationEvent";
				q.WithinInterval = new TimeSpan(0, 0, 3);
				q.Condition = @
				"TargetInstance ISA 'Win32_DiskDrive' ";
				//EventLog es una forma de escribir lo que está pasando en
				//el visor de sucesos de windows
				EventLog.WriteEntry(q.QueryString);
				w = new ManagementEventWatcher(q);
				w.EventArrived += new
				EventArrivedEventHandler(we.UsbEventArrived);
				w.Start();

			} catch (Exception e) {
				EventLog.WriteEntry(e.Message);
			}
		}
		protected override void OnStop() {

			// TODO: Add code here to perform any tear-down necessary to stop your service.
		}
		public void UsbEventArrived(object sender, EventArrivedEventArgs e) {
			ManagementBaseObject mbo = null;

			mbo = (ManagementBaseObject) e.NewEvent;

			if ((mbo.ClassPath.ClassName == "__InstanceCreationEvent")) {
				EventLog.WriteEntry("Usb insertado");
				try {
					//Busco todos los dispositivos USB del sistema
					SelectQuery selectQuery = new SelectQuery("SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB'");
					ManagementObjectSearcher searcher = new
					ManagementObjectSearcher(selectQuery);

					string archivoContenido = contenidoArchivo();
					//Verifico los que están insertados
					foreach(ManagementObject disk in searcher.Get()) {
						//Guardo en idUSB el serial del 
						string idUSB = disk["PNPDeviceID"].ToString();
						EventLog.WriteEntry("Encontrado dispositivo: " + idUSB);
						deshabilitarUSB(!estaEnLaLista(idUSB, archivoContenido), idUSB);
					}

				} catch (Exception j) {
					EventLog.WriteEntry(j.Message);
				}

			} else {
				EventLog.WriteEntry("Usb retirado");

			}
			Console.WriteLine((string) mbo["Name"]);

			foreach(PropertyData prop in mbo.Properties)
			EventLog.WriteEntry("{0} - {1}" + prop.Name + prop.Value);
		}
		public void deshabilitarUSB(bool deshabilitar, string idUSB) {
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.CreateNoWindow = false;
			startInfo.UseShellExecute = false;
			//Reenvía todo la salida de Devcon a mi aplicación c#
			startInfo.RedirectStandardOutput = true;

			//Devcon es la utilidad que permite desactivar hardware
			startInfo.FileName = "devcon.exe";

			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			//Deshabilito o habilito según sea el caso
			//Es necesario utilizar @ porque el idUSB suele contener carácteres especiales como \, &..
			startInfo.Arguments = ((deshabilitar) ? ("disable @") : ("enable @")) + idUSB;

			EventLog.WriteEntry(startInfo.FileName.ToString() + " " + startInfo.Arguments.ToString());

			try {
				using(Process exeProcess = Process.Start(startInfo)) {
					EventLog.WriteEntry(exeProcess.StandardOutput.ReadToEnd());
					exeProcess.WaitForExit();
				}
			} catch (Exception x) {
				EventLog.WriteEntry(x.Message);
			}

		}
		//Desemcriptar archivo
		static public string DecodeFrom64(string encodedData) {
			byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
			string returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
			return returnValue;
		}
		//Buscar items duplicados
		private bool estaEnLaLista(string valuestr, string mainstr) {
			int index = mainstr.IndexOf(valuestr);
			bool estado = (mainstr.IndexOf(valuestr) != -1);
			EventLog.WriteEntry("El dispositivo " + ((estado) ? ("si") : ("no")) + " está en la lista");
			return (estado) ? (true) : (false);
		}
		private string contenidoArchivo() {
			string s = "";
			using(StreamReader rdr = File.OpenText(Environment.GetEnvironmentVariable("windir").ToString() + "\\usb.b64"))
			s = rdr.ReadToEnd();
			return DecodeFrom64(s).Trim();
		}
	}
}