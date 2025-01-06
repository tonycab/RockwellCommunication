using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RockwellCommunication.Protocol;
using RockwellCommunication.IOSignals;
using SiiF_VISION.Technique.Communications;
using RockwellCommunication;
using System.Globalization;
using System.Reflection;

namespace Exemple
{

    public class LodgeEthernetIP : ICom
    {


        /// <summary>
        /// Changement d'état de la connection Modbus
        /// </summary>
        public event Action<object, EthernetIPEventArgs> EthernetIPStateEventArgs;
        public event Action<int, string, string> EvtTrigger2D;
        public event Action<string, string, string, bool> EvtTrigger3D;
        public event Action<string, string, string, string[]> EvtTrigger3DTable;
        public event Action<string[]> EvtConcat3DStart;
        public event Action<string[]> EvtConcat3DAdd;
        public event Action EvtConcat3DStop;
        public event Action<int, string, string> EvtInspectRegions;
        public event Action<string, string, string> EvtTrigger3Drapide;
        public event Action<string, string, string> EvtDataTrigger;
        public event Action<string, string> EvtLoadCampaign;
        public event Action EvtLoadFreeRunStart;
        public event Action EvtLoadFreeRunStop;
        public event Action<string[]> EvtCalibPose;
        public event Action EvtCalibStart;
        public event Action EvtCalibStop;
        public event Action<string, string> EvtReconstruct3DStart;
        public event Action<string, string> EvtReconstructInPos;
        public event Action EvtReconstructStop;
        public event Action<string, string, string, string[]> EvtReconstructAdd;
        public event Action<string, string, string> EvtCalibAxeExtInPos;
        public event Action EvtCalibAxeExtStart;
        public event Action EvtCalibAxeExtStop;
        public event Action EvtConnectCam;
        public event Action EvtDisconnectCam;
        public event Action EvtGetTempPhoxi;
        public event Action EvtGetVisionState;
        public event Action EvtStateAcknoledgement;
        public event Action EvtRepetabiliteStart;
        public event Action EvtRepetabiliteStop;
        public event Action EvtBackgroundLearning;


        public EthernetIP eip;


        public Action<long> EndReadWriteEnd;
        public Action<string> LogError;
        /// <summary>
        /// Returns the table number under the camera
        /// </summary>
        public short GetNumTable {  get => eip["IN.CAMERA1.NumTable"].Value;}

        /// <summary>
        /// Informe le PLC que la caméra 1 est en défaut
        /// </summary>
        public bool FaultCam1 { get => eip["OUT.CAMERA1.Fault"].Value; set => eip["OUT.CAMERA1.Fault"].Value = value; }


        /// <summary>
        /// Informe le PLC que la caméra 2 est en défaut
        /// </summary>
        public bool FaultCam2 { get => eip["OUT.CAMERA2.Fault"].Value; set => eip["OUT.CAMERA2.Fault"].Value = value; }


        public event Action<int> NumberTableChanged;

        public LodgeEthernetIP()
        {

            eip = new EthernetIP("192.32.98.50", "1.0", "Vision_Out", "Vision_In");
            eip.ImportUdtInput("UDT_VISION_OUT.L5X");
            eip.ImportUdtOutput("UDT_VISION_IN.L5X");

            //Event read write EIP end
            eip.EndReadWrite += (o)=>EndReadWriteEnd?.Invoke(o);

            //Event error
            eip.LogError += (o) => LogError?.Invoke(o);

            //Evennement changement d'etat de la connection Modbus
            eip.EthernetIPStateEventArgs += EthernetIPStateEvent;

            Inputs = eip.Inputs;
            Outputs = eip.Outputs;

            //Bit de vie communication
            eip["IN.GENERALITY.Life"].SignalChanged += (s) =>
            {
                eip["OUT.GENERALITY.LifeEcho"].Value = s.Value;
            };

            //Numero de table devant la caméra 2
            eip["IN.CAMERA2.NumTable"].SignalChanged += (o) => NumberTableChanged.Invoke(o.Value);

            //Trigger camera 1
            eip["IN.CAMERA1.Trigg"].SignalChanged += (s) =>
            {
                Task.Run(() =>
                {
                    if (s.Value == true)
                    {
                       
                        eip["OUT.CAMERA1.Data"].Value = false;

                        EvtTrigger3DTable?.Invoke(eip["IN.CAMERA1.TicketPart1"].Value.ToString(), eip["IN.CAMERA1.Prog"].Value.ToString(), eip["IN.CAMERA1.Model"].Value.ToString(), new string[] { "Cam1" });
                        
                        eip["OUT.CAMERA1.EndTrig"].Value = true;
                    }
                    else
                    {
                        eip["OUT.CAMERA1.EndTrig"].Value = false;
                    }
                });

            };

            //Trigger camera 2
            eip["IN.CAMERA2.Trigg"].SignalChanged += (s) =>
            {
                Task.Run(() =>
                {
                    if (s.Value == true)
                    {

                        eip["OUT.CAMERA2.Data"].Value = false;

                        EvtTrigger3DTable?.Invoke(eip["IN.CAMERA2.TicketPart1"].Value.ToString(), eip["IN.CAMERA2.Prog"].Value.ToString(), eip["IN.CAMERA2.Model"].Value.ToString(), new string[] { "Cam2" });

                        eip["OUT.CAMERA2.EndTrig"].Value = true;
                    }
                    else
                    {
                        eip["OUT.CAMERA2.EndTrig"].Value = false;
                    }
                });

            };

            //Acq data caméra 1
            eip["IN.CAMERA1.AcqData"].SignalChanged += (s) =>
            {
                if (s.Value == true)
                {
                    eip["OUT.CAMERA1.PART1.Ticket"].Value = 0;
                    eip["OUT.CAMERA1.DataPrg"].Value = 0;
                    eip["OUT.CAMERA1.DataModel"].Value = 0;
                    eip["OUT.CAMERA1.PART1.State"].Value = 0;
                    eip["OUT.CAMERA1.PART1.Score"].Value = 0;
                    eip["OUT.CAMERA1.PART1.X"].Value = 0;
                    eip["OUT.CAMERA1.PART1.Y"].Value = 0;
                    eip["OUT.CAMERA1.PART1.Z"].Value = 0;
                    eip["OUT.CAMERA1.PART1.Rx"].Value = 0;
                    eip["OUT.CAMERA1.PART1.Ry"].Value = 0;
                    eip["OUT.CAMERA1.PART1.Rz"].Value = 0;

                    //Result cam 1 part 1 attacks
                    eip["OUT.CAMERA1.PART1.ATTACK.NUMBER_ATTACK"].Value = 0;
                    for (int i = 0; i < 19; i++)
                    {
                        eip[$"OUT.CAMERA1.PART1.ATTACK.POSITION[{i}].X"].Value = 0;
                        eip[$"OUT.CAMERA1.PART1.ATTACK.POSITION[{i}].Y"].Value = 0;
                        eip[$"OUT.CAMERA1.PART1.ATTACK.POSITION[{i}].Z"].Value = 0;
                    }

                    //Result cam1 part 2
                    eip["OUT.CAMERA1.PART2.Ticket"].Value = 0;
                    eip["OUT.CAMERA1.PART2.Ticket"].Value = 0;
                    eip["OUT.CAMERA1.PART2.State"].Value = 0;
                    eip["OUT.CAMERA1.PART2.Score"].Value = 0;
                    eip["OUT.CAMERA1.PART2.X"].Value = 0;
                    eip["OUT.CAMERA1.PART2.Y"].Value = 0;
                    eip["OUT.CAMERA1.PART2.Z"].Value = 0;
                    eip["OUT.CAMERA1.PART2.Rx"].Value = 0;
                    eip["OUT.CAMERA1.PART2.Ry"].Value = 0;
                    eip["OUT.CAMERA1.PART2.Rz"].Value =0;


                    //Result cam 1 part 2 attacks
                    eip["OUT.CAMERA1.PART2.ATTACK.NUMBER_ATTACK"].Value =0;
                    for (int i = 0; i < 19; i++)
                    {
                        eip[$"OUT.CAMERA1.PART2.ATTACK.POSITION[{i}].X"].Value = 0;
                        eip[$"OUT.CAMERA1.PART2.ATTACK.POSITION[{i}].Y"].Value = 0;
                        eip[$"OUT.CAMERA1.PART2.ATTACK.POSITION[{i}].Z"].Value = 0;
                    }

                    eip["OUT.CAMERA1.Data"].Value = false;
                }};

            //Acq data caméra 2
            eip["IN.CAMERA2.AcqData"].SignalChanged += (s) =>
            {
                if (s.Value == true)
                {
                    eip["OUT.CAMERA2.PART1.Ticket"].Value = 0;
                    eip["OUT.CAMERA2.DataPrg"].Value = 0;
                    eip["OUT.CAMERA2.DataModel"].Value = 0;
                    eip["OUT.CAMERA2.PART1.State"].Value = 0;
                    eip["OUT.CAMERA2.PART1.Score"].Value = 0;
                    eip["OUT.CAMERA2.PART1.X"].Value = 0;
                    eip["OUT.CAMERA2.PART1.Y"].Value = 0;
                    eip["OUT.CAMERA2.PART1.Z"].Value = 0;
                    eip["OUT.CAMERA2.PART1.Rx"].Value = 0;
                    eip["OUT.CAMERA2.PART1.Ry"].Value = 0;
                    eip["OUT.CAMERA2.PART1.Rz"].Value = 0;

                    //Result cam 2 part 1 attacks
                    eip["OUT.CAMERA2.PART1.ATTACK.NUMBER_ATTACK"].Value = 0;
                    for (int i = 0; i < 19; i++)
                    {
                        eip[$"OUT.CAMERA2.PART1.ATTACK.POSITION[{i}].X"].Value = 0;
                        eip[$"OUT.CAMERA2.PART1.ATTACK.POSITION[{i}].Y"].Value = 0;
                        eip[$"OUT.CAMERA2.PART1.ATTACK.POSITION[{i}].Z"].Value = 0;
                    }

                    //Result cam2 part 2
                    eip["OUT.CAMERA2.PART2.Ticket"].Value = 0;
                    eip["OUT.CAMERA2.PART2.State"].Value = 0;
                    eip["OUT.CAMERA2.PART2.Score"].Value = 0;
                    eip["OUT.CAMERA2.PART2.X"].Value = 0;
                    eip["OUT.CAMERA2.PART2.Y"].Value = 0;
                    eip["OUT.CAMERA2.PART2.Z"].Value = 0;
                    eip["OUT.CAMERA2.PART2.Rx"].Value = 0;
                    eip["OUT.CAMERA2.PART2.Ry"].Value = 0;
                    eip["OUT.CAMERA2.PART2.Rz"].Value = 0;


                    //Result cam 2 part 2 attacks
                    eip["OUT.CAMERA2.PART2.ATTACK.NUMBER_ATTACK"].Value = 0;
                    for (int i = 0; i < 19; i++)
                    {
                        eip[$"OUT.CAMERA2.PART2.ATTACK.POSITION[{i}].X"].Value = 0;
                        eip[$"OUT.CAMERA2.PART2.ATTACK.POSITION[{i}].Y"].Value = 0;
                        eip[$"OUT.CAMERA2.PART2.ATTACK.POSITION[{i}].Z"].Value = 0;
                    }

                    eip["OUT.CAMERA2.Data"].Value = false;
                }
            };

            //Calibration camera1
            eip["IN.CAMERA1.StartCalib"].SignalChanged += (s) =>
            {
                
            };
            //Calibration camera2
            eip["IN.CAMERA2.StartCalib"].SignalChanged += (s) =>
            {
                
            };
        }


        public void SendMessage(string message)
        {
            string[] m = message.Split(';');

            //Result trigger Camera 1
            if (m[0] == "/trigger" && int.Parse(m[1]) > 0 && int.Parse(m[1]) < 10000)
            {
                try
                {
                    //Result cam 1
                    eip["OUT.CAMERA1.PART1.Ticket"].Value = ushort.Parse(m[1]);
                    eip["OUT.CAMERA1.DataPrg"].Value = ushort.Parse(m[2]);
                    eip["OUT.CAMERA1.DataModel"].Value = ushort.Parse(m[3]);
                    eip["OUT.CAMERA1.PART1.State"].Value = m[4] == "PASS" ? (short)1 : (short)0;
                    eip["OUT.CAMERA1.PART1.Score"].Value = (short)float.Parse(m[5]);

                    //Result cam1 part 1
                    eip["OUT.CAMERA1.PART1.X"].Value = float.Parse(m[6]);
                    eip["OUT.CAMERA1.PART1.Y"].Value = float.Parse(m[7]);
                    eip["OUT.CAMERA1.PART1.Z"].Value = float.Parse(m[8]);
                    eip["OUT.CAMERA1.PART1.Rx"].Value = float.Parse(m[9]);
                    eip["OUT.CAMERA1.PART1.Ry"].Value = float.Parse(m[10]);
                    eip["OUT.CAMERA1.PART1.Rz"].Value = float.Parse(m[11]);

                    //Result cam 1 part 1 attacks
                    eip["OUT.CAMERA1.PART1.ATTACK.NUMBER_ATTACK"].Value = int.Parse(m[12]);
                    for (int i = 0; i < int.Parse(m[12]); i++)
                    {
                        eip[$"OUT.CAMERA1.PART1.ATTACK.POSITION[{i}].X"].Value = double.Parse(m[13 + (uint)i * 3]);
                        eip[$"OUT.CAMERA1.PART1.ATTACK.POSITION[{i}].Y"].Value = double.Parse(m[14 + (uint)i * 3]);
                        eip[$"OUT.CAMERA1.PART1.ATTACK.POSITION[{i}].Z"].Value = double.Parse(m[15 + (uint)i * 3]);
                    }

                    //Result cam1 part 2
                    eip["OUT.CAMERA1.PART2.X"].Value = float.Parse(m[6]);
                    eip["OUT.CAMERA1.PART2.Y"].Value = float.Parse(m[7]);
                    eip["OUT.CAMERA1.PART2.Z"].Value = float.Parse(m[8]);
                    eip["OUT.CAMERA1.PART2.Rx"].Value = float.Parse(m[9]);
                    eip["OUT.CAMERA1.PART2.Ry"].Value = float.Parse(m[10]);
                    eip["OUT.CAMERA1.PART2.Rz"].Value = float.Parse(m[11]);


                    //Result cam 1 part 2 attacks
                    eip["OUT.CAMERA1.PART2.ATTACK.NUMBER_ATTACK"].Value = int.Parse(m[12]);
                    for (int i = 0; i < int.Parse(m[12]); i++)
                    {
                    eip[$"OUT.CAMERA1.PART2.ATTACK.POSITION[{i}].X"].Value = double.Parse(m[13 + (uint)i * 3]);
                    eip[$"OUT.CAMERA1.PART2.ATTACK.POSITION[{i}].Y"].Value = double.Parse(m[14 + (uint)i * 3]);
                    eip[$"OUT.CAMERA1.PART2.ATTACK.POSITION[{i}].Z"].Value = double.Parse(m[15 + (uint)i * 3]);
                    }

                    //Result ready for read for the PLC
                    eip["OUT.CAMERA1.Data"].Value = true;

                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);

                }
            }


            //Result trigger Camera 2
            if (m[0] == "/trigger" && int.Parse(m[1]) > 20000 && int.Parse(m[1]) < 30000)
            {

                try
                {
                    //Result cam 2
                    eip["OUT.CAMERA2.PART1.Ticket"].Value = ushort.Parse(m[1]);
                    eip["OUT.CAMERA2.DataPrg"].Value = ushort.Parse(m[2]);
                    eip["OUT.CAMERA2.DataModel"].Value = ushort.Parse(m[3]);
                    eip["OUT.CAMERA2.PART1.State"].Value = m[4] == "PASS" ? (short)1 : (short)0;
                    eip["OUT.CAMERA2.PART1.Score"].Value = (short)float.Parse(m[5]);

                    //Result cam2 part 1
                    eip["OUT.CAMERA2.PART1.X"].Value = float.Parse(m[6]);
                    eip["OUT.CAMERA2.PART1.Y"].Value = float.Parse(m[7]);
                    eip["OUT.CAMERA2.PART1.Z"].Value = float.Parse(m[8]);
                    eip["OUT.CAMERA2.PART1.Rx"].Value = float.Parse(m[9]);
                    eip["OUT.CAMERA2.PART1.Ry"].Value = float.Parse(m[10]);
                    eip["OUT.CAMERA2.PART1.Rz"].Value = float.Parse(m[11]);

                    //Result cam 2 part 1 attacks
                    eip["OUT.CAMERA2.PART1.ATTACK.NUMBER_ATTACK"].Value = int.Parse(m[12]);
                    for (int i = 0; i < int.Parse(m[12]); i++)
                    {
                        eip[$"OUT.CAMERA2.PART1.ATTACK.POSITION[{i}].X"].Value = double.Parse(m[13 + (uint)i * 3]);
                        eip[$"OUT.CAMERA2.PART1.ATTACK.POSITION[{i}].Y"].Value = double.Parse(m[14 + (uint)i * 3]);
                        eip[$"OUT.CAMERA2.PART1.ATTACK.POSITION[{i}].Z"].Value = double.Parse(m[15 + (uint)i * 3]);
                    }

                    //Result cam2 part 2
                    eip["OUT.CAMERA2.PART2.X"].Value = float.Parse(m[6]);
                    eip["OUT.CAMERA2.PART2.Y"].Value = float.Parse(m[7]);
                    eip["OUT.CAMERA2.PART2.Z"].Value = float.Parse(m[8]);
                    eip["OUT.CAMERA2.PART2.Rx"].Value = float.Parse(m[9]);
                    eip["OUT.CAMERA2.PART2.Ry"].Value = float.Parse(m[10]);
                    eip["OUT.CAMERA2.PART2.Rz"].Value = float.Parse(m[11]);


                    //Result cam 2 part 2 attacks
                    eip["OUT.CAMERA2.PART2.ATTACK.NUMBER_ATTACK"].Value = int.Parse(m[12]);
                    for (int i = 0; i < int.Parse(m[12]); i++)
                    {
                        eip[$"OUT.CAMERA2.PART2.ATTACK.POSITION[{i}].X"].Value = double.Parse(m[13 + (uint)i * 3]);
                        eip[$"OUT.CAMERA2.PART2.ATTACK.POSITION[{i}].Y"].Value = double.Parse(m[14 + (uint)i * 3]);
                        eip[$"OUT.CAMERA2.PART2.ATTACK.POSITION[{i}].Z"].Value = double.Parse(m[15 + (uint)i * 3]);
                    }

                    //Result ready for read for the PLC
                    eip["OUT.CAMERA2.Data"].Value = true;

                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);

                }
















            }

        }


        public string IP
        {
            get
            {
                return eip?.IPadressPlc;
            }
            set
            {
                if (eip != null)
                {
                    eip.IPadressPlc = value;
                }
            }
        }

        /// <summary>
        /// Signaux d'entrées
        /// </summary>
        public ThreadedBindingList<Signal> Inputs { get; set; }

        /// <summary>
        /// Signaux de sorties
        /// </summary>
        public ThreadedBindingList<Signal> Outputs { get; set; }


        private void EthernetIPStateEvent(object arg1, EthernetIPEventArgs arg2)
        {
            EthernetIPStateEventArgs?.Invoke(arg1, arg2);
        }
        public void Start()
        {
            eip?.Start();
        }

        public void Stop()
        {
            eip?.Stop();
        }

    


        public bool OfflineMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
