using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONConfFileEditor.Models
{
	public enum HardwareType
	{
		Demo = 0,
		HarpiaMainBoardSingleShutter = 1,
		HarmonicsModuleOnCarbide_v4_8 = 2,
	}

	public class HardwareComponentConfiguration
	{
		[JsonConverter(typeof(StringEnumConverter))]
		public HardwareType HardwareType { get; set; } = HardwareType.Demo;
	}

	class HarpiaMainBoardSingleShutterConfig : HardwareComponentConfiguration
	{
		public int ShutterInUseIndex { get; set; } = 0;
	}

	public class HarmonicsModuleOnCarbide_V4_8 : HardwareComponentConfiguration
	{
		public int ShutterInUseIndex { get; set; } = 0;
		public int ShutterInUseIndex2 { get; set; } = 0;
	}

	class CarbideSIModel
	{		
		public HardwareType HardwareType { get; set; } = HardwareType.HarpiaMainBoardSingleShutter;

		public int HarpiaMainBoardSingleShutterStageIndex { get; set; } = 0;//todo this prop should be moved to object HardwareConfig, which is specific to hardware


		//public List<HardwareComponentConfiguration> HardwareComponents { get; set; } = new List<HardwareComponentConfiguration>();

		public string CarbideIPAddress { get; set; } = "";
	
		public CarbideElectronicsType CarbideElectronicsType { get; set; } = CarbideElectronicsType.GHI;
		public MainOutputConf MainOutput { get; set; } = new MainOutputConf();
		public UnCompressedAfterPPConf UnCompressedAfterPP { get; set; } = new UnCompressedAfterPPConf();
		public UnCompressedConf UnCompressed { get; set; } = new UnCompressedConf();
		public SimultanuesOscOutput SimultanuesOscOutput { get; set; } = new SimultanuesOscOutput();
		public AutomatedOscOutput AutomatedOscOutput { get; set; } = new AutomatedOscOutput();

		public ExternalPPConfiguration ExternalPP { get; set; } = new ExternalPPConfiguration();
		public ExternalPPConfiguration SecondExternalPP { get; set; } = new ExternalPPConfiguration();
		public bool IsManufacturingMode { get; set; } = false;
	}

	public enum CarbideElectronicsType
	{
		GHI,
		Toradex
	}

	public class AttenuatorConf
	{
		public bool IsPresent { get; set; }
		//todo maybe diode address
		public double DiodeOffset { get; set; }
		public double DiodeGain { get; set; }
	}

	public class MainOutputConf
	{
		public AttenuatorConf Attenuator { get; set; } = new AttenuatorConf();
		public AttenuatorConf SecondaryAttenuator { get; set; } = new AttenuatorConf();
		//public List<PPDividerEntry> PPDividers { get; set; } = new List<PPDividerEntry>();
	}

	public class UnCompressedAfterPPConf
	{
		public bool IsPresent { get; set; }
	}

	public class UnCompressedConf
	{
		public bool IsPresent { get; set; }
		/// <summary>
		/// ratio between main output power and uncompressed output power is fixed, but PP divider does not affect uncompressed output
		/// </summary>
		public double PowerMultiplier { get; set; }
		public AttenuatorConf Attenuator { get; set; } = new AttenuatorConf();
	}
	public class SimultanuesOscOutput
	{
		public bool IsPresent { get; set; }
		public double OutputPowerCalibrationGain { get; set; }
		public double OutputPowerCalibrationOffset { get; set; }
	}

	public class AutomatedOscOutput
	{
		public bool IsPresent { get; set; }
		public double OutputPowerCalibrationGain { get; set; }
		public double OutputPowerCalibrationOffset { get; set; }
	}

	public class ExternalPPConfiguration
	{
		public bool IsPresent { get; set; }
		//public List<PPDividerEntry> PPDividers { get; set; } = new List<PPDividerEntry>();
		public double NominalPPVoltage { get; set; } = 300;
		public double PPOffDelayNs { get; set; } = 0d;

		public bool IsInverted { get; set; } = false;

		[JsonConverter(typeof(StringEnumConverter))]
		public PPHardwareType HardwareType { get; set; } = PPHardwareType.CarbideHVPP;
		public int SyncBoxChannelNo { get; set; } = 1;
		public string CustomTitle { get; set; } = "";

	}

	public enum PPHardwareType
	{
		Pharos,
		CarbideHVRA,
		CarbideHVPP,
		[Obsolete]
		Carbide
	}



	public class PPDividerEntry
	{
		public int Divider { get; set; } = 1;
	}


}
