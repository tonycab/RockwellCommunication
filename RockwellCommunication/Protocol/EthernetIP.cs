using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RockwellCommunication.IOSignals;
using System.Threading;
using libplctag;
using libplctag.DataTypes;
using System.Data;
using System.Diagnostics;

namespace RockwellCommunication.Protocol
{
     

    public class EthernetIP : Dictionary<string, Signal>
    {

    


        #region Properties public

        /// <summary>
        /// Liste des signaux d'entrées
        /// </summary>
        public ThreadedBindingList<Signal> Inputs { get; set; }

        /// <summary>
        /// Liste des signaux de sorties
        /// </summary>
        public ThreadedBindingList<Signal> Outputs { get; set; }


        /// <summary>
        /// Adresse IP du PLC
        /// </summary>
        public string IPadressPlc { get; set; }

        /// <summary>
        /// Timeout de connection
        /// </summary>
        public int TimeOutConnection { get; set; } = 1000;


        /// <summary>
        /// Adresse de l'emplacement mémoire du PLC
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Type de PLC
        /// </summary>
        public PlcType PlcType { get; set; }


        

        /// <summary>
        /// Tag de départ des signaux d'entrées
        /// </summary>
        public string NameTagInput { get; set; }

        /// <summary>
        /// Tag de départ des signaux de sorties
        /// </summary>
        public string NameTagOutput { get; set; }

        /// <summary>
        /// Nombre de %MW des siganux d'entrées
        /// </summary>
        public int SizeRegisterInput { get; set; } = 2048;
        /// <summary>
        /// Nombre de %MW des signaux de sorties
        /// </summary>
        public int SizeRegisterOutput { get; set; } = 2048;


        /// <summary>
        /// Nombre de %MW des siganux d'entrées
        /// </summary>
        public int ElementCountInput { get; set; } = 1;
        /// <summary>
        /// Nombre de %MW des signaux de sorties
        /// </summary>
        public int ElementCountOutput { get; set; } = 1;


        
        /// <summary>
        /// Evenement de changement d'état de la connection Modbus
        /// </summary>
        public event Action<object, EthernetIPEventArgs> EthernetIPStateEventArgs;


        public event Action<long> EndReadWrite;

        public event Action<string> LogError;


        #endregion

        #region Properties private

        private byte[] RegistreInput;

        private byte[] RegisterOutput;


        private CancellationTokenSource cancellationToken;

        #endregion

        #region Constructor 
        /// <summary>
        /// Constructeur d'une instance de communication modbus TCP master
        /// </summary>
        /// <param name="ipAddressPlc">Adresse IP du PLC</param>
        /// <param name="portPlc">Port de communication du PLC</param>
        /// <param name="startRegisterInput">%MW de départ des signaux d'entrées</param>
        /// <param name="sizeRegisterInput">%MW de départ des signaux de sorties</param>
        /// <param name="startRegisterOutput">Nombre de %MW des siganux d'entrées</param>
        /// <param name="sizeRegisterOutput">Nombre de %MW des signaux de sorties</param>
        public EthernetIP(string ipAddressPlc, string path, string nameTagInput = "", string nameTagOutput = "")
        {

            IPadressPlc = ipAddressPlc;
            Path = path;
            PlcType = PlcType.ControlLogix;

            NameTagInput = nameTagInput;
            NameTagOutput = nameTagOutput;

            RegistreInput = new byte[SizeRegisterInput];
            RegisterOutput = new byte[SizeRegisterOutput];

            Inputs = new ThreadedBindingList<Signal>();
            Outputs = new ThreadedBindingList<Signal>();

        }

        #endregion

        #region Add/Remove signals

        /// <summary>
        /// Add signal input
        /// </summary>
        /// <param name="signal"></param>
        public void AddSignalInput(Signal signal)
        {

            this.Add(signal.Name, signal);

            Inputs.Add(signal);
        }
        /// <summary>
        /// Add signal output
        /// </summary>
        /// <param name="signal"></param>
        public void AddSignalOutput(Signal signal)
        {

            this.Add(signal.Name, signal);

            Outputs.Add(signal);
        }

        /// <summary>
        /// Add input signal type bool
        /// </summary>
        /// <param name="signalInput">Signal d'entrée</param>
        public Signal<bool> AddSignalInputBool(string name, string designation, uint numberByte, int numberBit)
        {

            Signal<bool> b = new Signal<bool>(name, designation, numberByte, numberBit);

            this.Add(b.Name, b);

            Inputs.Add(b);

            return b;

        }

        /// <summary>
        /// Add input signal type byte
        /// </summary>
        /// <param name="signalInput">Signal d'entrée</param>
        public Signal<byte> AddSignalInputByte(string name, string designation, uint numberByte)
        {

            Signal<byte> b = new Signal<byte>(name, designation, numberByte);

            this.Add(b.Name, b);

            Inputs.Add(b);

            return b;

        }

        /// <summary>
        /// Add input signal type sbyte
        /// </summary>
        /// <param name="signalInput">Signal d'entrée</param>
        public Signal<sbyte> AddSignalInputSByte(string name, string designation, uint numberByte)
        {

            Signal<sbyte> b = new Signal<sbyte>(name, designation, numberByte);

            this.Add(b.Name, b);

            Inputs.Add(b);

            return b;

        }

        /// <summary>
        /// Add input signal type short
        /// </summary>
        /// <param name="signalInput">Signal d'entrée</param>
        public Signal<short> AddSignalInputShort(string name, string designation, uint numberByte)
        {

            Signal<short> b = new Signal<short>(name, designation, numberByte);

            this.Add(b.Name, b);

            Inputs.Add(b);

            return b;

        }

        /// <summary>
        /// Add input signal type ushort
        /// </summary>
        /// <param name="signalInput">Signal d'entrée</param>
        public Signal<ushort> AddSignalInputUShort(string name, string designation, uint numberByte)
        {

            Signal<ushort> b = new Signal<ushort>(name, designation, numberByte);

            this.Add(b.Name, b);

            Inputs.Add(b);

            return b;

        }

        /// <summary>
        /// Add input signal type int
        /// </summary>
        /// <param name="signalInput">Signal d'entrée</param>
        public Signal<int> AddSignalInputInt(string name, string designation, uint numberByte)
        {

            Signal<int> b = new Signal<int>(name, designation, numberByte);

            this.Add(b.Name, b);

            Inputs.Add(b);

            return b;

        }

        /// <summary>
        /// Add input signal type uint
        /// </summary>
        /// <param name="signalInput">Signal d'entrée</param>
        public Signal<uint> AddSignalInputUInt(string name, string designation, uint numberByte)
        {

            Signal<uint> b = new Signal<uint>(name, designation, numberByte);

            this.Add(b.Name, b);

            Inputs.Add(b);

            return b;

        }

        /// <summary>
        /// Add input signal type float
        /// </summary>
        /// <param name="signalInput">Signal d'entrée</param>
        public Signal<float> AddSignalInputFloat(string name, string designation, uint numberByte)
        {

            Signal<float> b = new Signal<float>(name, designation, numberByte);

            this.Add(b.Name, b);

            Inputs.Add(b);

            return b;

        }

        /// <summary>
        /// Remove signal insput
        /// </summary>
        /// <param name="name"></param>
        public void RemoveSignalInput(string name)
        {
            Inputs.Remove((Signal)this[name]);
            this.Remove(name);
        }



        /// <summary>
        /// Remove output signal type bool
        /// </summary>
        /// <param name="signalOutput"></param>
        public Signal<bool> AddSignalOutputBool(string name, string designation, uint numberByte, int numberBit)
        {

            Signal<bool> b = new Signal<bool>(name, designation, numberByte, numberBit);

            this.Add(b.Name, b);

            Outputs.Add(b);

            return b;

        }

        /// <summary>
        /// Remove output signal type byte
        /// </summary>
        /// <param name="signalOutput">Signal d'sortie</param>
        public Signal<byte> AddSignalOutputByte(string name, string designation, uint numberByte)
        {

            Signal<byte> b = new Signal<byte>(name, designation, numberByte);

            this.Add(b.Name, b);

            Outputs.Add(b);

            return b;

        }

        /// <summary>
        /// Remove output signal type sbyte
        /// /// </summary>
        /// <param name="signalOutput">Signal d'sortie</param>
        public Signal<sbyte> AddSignalOutputSByte(string name, string designation, uint numberByte)
        {

            Signal<sbyte> b = new Signal<sbyte>(name, designation, numberByte);

            this.Add(b.Name, b);

            Outputs.Add(b);

            return b;

        }

        /// <summary>
        /// Remove output signal type short
        /// </summary>
        /// <param name="signalOutput">Signal d'sortie</param>
        public Signal<short> AddSignalOutputShort(string name, string designation, uint numberByte)
        {

            Signal<short> b = new Signal<short>(name, designation, numberByte);

            this.Add(b.Name, b);

            Outputs.Add(b);

            return b;

        }

        /// <summary>
        /// Remove output signal type ushort
        /// </summary>
        /// <param name="signalOutput">Signal d'sortie</param>
        public Signal<ushort> AddSignalOutputUShort(string name, string designation, uint numberByte)
        {

            Signal<ushort> b = new Signal<ushort>(name, designation, numberByte);

            this.Add(b.Name, b);

            Outputs.Add(b);

            return b;

        }

        /// <summary>
        /// Remove output signal type int
        /// </summary>
        /// <param name="signalOutput">Signal d'sortie</param>
        public Signal<int> AddSignalOutputInt(string name, string designation, uint numberByte)
        {

            Signal<int> b = new Signal<int>(name, designation, numberByte);

            this.Add(b.Name, b);

            Outputs.Add(b);

            return b;

        }

        /// <summary>
        /// Remove output signal type uint
        /// </summary>
        /// <param name="signalOutput">Signal d'sortie</param>
        public Signal<uint> AddSignalOutputUInt(string name, string designation, uint numberByte)
        {

            Signal<uint> b = new Signal<uint>(name, designation, numberByte);

            this.Add(b.Name, b);

            Outputs.Add(b);

            return b;

        }

        /// <summary>
        /// Remove output signal type float
        /// </summary>
        /// <param name="signalOutput">Signal d'sortie</param>
        public Signal<float> AddSignalOutputFloat(string name, string designation, uint numberByte)
        {

            Signal<float> b = new Signal<float>(name, designation, numberByte);

            this.Add(b.Name, b);

            Outputs.Add(b);

            return b;

        }

        /// <summary>
        /// Remove output signal
        /// </summary>
        /// <param name="name">Signal de sortie</param>
        public void RemoveSignalOutput(string name)
        {
            Outputs.Remove((Signal)this[name]);
            this.Remove(name);
        }

        #endregion

        #region Start /Stop
        /// <summary>
        /// Start connection to PLC
        /// </summary>
        public void Start()
        {
            cancellationToken = new CancellationTokenSource();
            StartModbus();
        }
        /// <summary>
        /// Stop connection to PLC
        /// </summary>
        public void Stop()
        {
            cancellationToken?.Cancel();
        }

        #endregion

        #region Private Methode

        private Tag Input;

        private Tag Output;


        //Connection a l'automate
        private bool ConnectionClient()
        {
            try
            {
                Input = new Tag()
                {
                    Name = NameTagInput,
                    Gateway = IPadressPlc,
                    Path = "1,0",
                    PlcType = PlcType.ControlLogix,
                    Protocol = libplctag.Protocol.ab_eip,
                    ElementCount = ElementCountInput,
                };

                Output = new Tag()
                {
                    Name = NameTagOutput,
                    Gateway = IPadressPlc,
                    Path = "1,0",
                    PlcType = PlcType.ControlLogix,
                    Protocol = libplctag.Protocol.ab_eip,
                    ElementCount = ElementCountOutput,
                };

                Input.Initialize();
                Output.Initialize();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        //Cycle de connection et d'échange PC et PLC
        private void StartModbus()
        {
            Stopwatch stopWatch = new Stopwatch();

            Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {

                        //Connection au PLC
                        while (!ConnectionClient() & !cancellationToken.IsCancellationRequested)
                        {
                            EthernetIPStateEventArgs?.Invoke(this, new EthernetIPEventArgs(StateConnect.NoClientExist, IPadressPlc));
                            Thread.Sleep(5000);
                        }

                        //Etat connecté
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            System.Diagnostics.Debug.WriteLine("Connection success");

                            EthernetIPStateEventArgs?.Invoke(this, new EthernetIPEventArgs(StateConnect.Connected, IPadressPlc));
                        }

                        //Boucle de lecture / écriture sur le PLC
                        while (!cancellationToken.IsCancellationRequested)
                        {
                            stopWatch.Reset();
                            stopWatch.Start();

                            try
                            {
                                //Lecture des signaus d'entrée
                                Input.Read();
                                RegistreInput = Input.GetBuffer();

                            }
                            catch (Exception ex)
                            {
                                LogError?.Invoke($"{ex.Message}");
                                throw;
                            }


                            foreach (Signal signal in Inputs)
                            {
                                try
                                {
                                    signal.SetValue(RegistreInput);
                                }
                                catch (Exception ex)
                                {
                                    LogError?.Invoke($"{signal.Name} - {ex.Message}");
                                }
                            }

                            foreach (Signal signal in Inputs)
                            {
                                try
                                {
                                    signal.CallEventStateChanged();
                                }
                                catch (Exception ex)
                                {
                                    LogError?.Invoke($"{signal.Name} - {ex.Message}");
                                }
                            }

                            //Ecriture des signaux de sortie
                            foreach (Signal signal in Outputs)
                            {
                                try
                                {
                                    signal.SetRegister(RegisterOutput);
                                }
                                catch (Exception ex)
                                {
                                    LogError?.Invoke($"{signal.Name} - {ex.Message}");
                                }

                            }
                            try
                            {
                                Output.SetBuffer(RegisterOutput);
                                Output.Write();
                            }
                            catch (Exception ex)
                            {
                                LogError?.Invoke($"{ex.Message}");
                                throw;
                            }

                            Thread.Sleep(10);

                            stopWatch.Stop();

                            EndReadWrite?.Invoke(stopWatch.ElapsedMilliseconds);


                        }

                        //Connection arrêter
                        EthernetIPStateEventArgs?.Invoke(this, new EthernetIPEventArgs(StateConnect.Stopped, IPadressPlc));

                        Input.Dispose();
                        Output.Dispose();

                    }

                    catch (Exception ex)
                    {
                        LogError?.Invoke($"Error connection");

                        //Deconnection
                        EthernetIPStateEventArgs?.Invoke(this, new EthernetIPEventArgs(StateConnect.Disconnected, IPadressPlc));
                        Input.Dispose();
                        Output.Dispose();

                    }

                }
            });
        }

        public void ImportUdtInput(string path)
        {

            UDT_Manager uDT_Manager = UDT_Manager.LoadToXml(path);

            int byteAdress = 0;
            int bitAdress = 0;

            List<Signal> s = new List<Signal>();


            var sizeUdt = ResolveUdt(s, uDT_Manager.listUdt[uDT_Manager.UdtName], uDT_Manager, ref byteAdress, ref bitAdress, "IN");

            foreach (var signal in s)
            {
                this.AddSignalInput(signal);

            }

            this.SizeRegisterInput = sizeUdt;

        }
        public void ImportUdtOutput(string path)
        {

            UDT_Manager uDT_Manager = UDT_Manager.LoadToXml(path);

            int byteAdress = 0;
            int bitAdress = 0;

            List<Signal> s = new List<Signal>();


            var sizeUdt = ResolveUdt(s, uDT_Manager.listUdt[uDT_Manager.UdtName], uDT_Manager, ref byteAdress, ref bitAdress, "OUT");

            foreach (var signal in s)
            {
                this.AddSignalOutput(signal);

            }

            this.SizeRegisterInput = sizeUdt;

        }


        private int ResolveUdt(List<Signal> listVar, DataType NodeDataType, UDT_Manager udt_Manager, ref int byteAdress, ref int bitAdress, string namePreview)
        {

            //Regle comprise

            //Un UDT contenant au moins un type atomique doit faire minimum 4 bytes
            //Un UDT contenant aucun type atomique que des types UDT ne contient pas de bytes
            //Un bit déclaré alloue 2 bytes  qui sont complété
            //Un UDT démarre sur une adresse modulo de 4
            //un INT démarre sur une adresse modulo de 2
            //un FLOAT démarre sur une adresse modulo de 4

            string typePrecedent = "";
            int bitAllocation = 0;
            int sizeUdt = 0;
            int sizeUSINTForBool = 0;


            //Iteration des types
            foreach (var type in NodeDataType.Types)
            {


                int a;
                if (type.Dimension == 0) { a = 0; }
                else
                {
                    a = type.Dimension - 1;
                }

                //Iteration sur les dimension 
                for (int i = 0; i <= a; i++)
                {

                    string ArrayString = "";

                    if (type.Dimension > 0)
                    {
                        ArrayString = $"[{i}]";
                    }


                    if (typePrecedent != type.Type)
                    {


                        //Complete la taille de l'udt de 1 byte si le précédent est un bit
                        if (sizeUSINTForBool > 0 & sizeUSINTForBool < 2)
                        {
                            byteAdress += 2 - sizeUSINTForBool;
                            sizeUdt += 1;
                            sizeUSINTForBool = 0;
                        }

                        if (bitAdress > 0)
                        {
                            bitAdress = 0;
                            byteAdress += 1;

                        }

                    }

                    switch (type.Type)
                    {

                        case "BIT":


                            if (sizeUSINTForBool == 0) sizeUSINTForBool = 1;


                            if (type.Hidden == false)
                            {
                                listVar.Add(new Signal<bool>($"{namePreview}.{type.Name}", "", (uint)byteAdress, bitAdress));
                                bitAdress += 1;
                            }

                            if (bitAdress > 7)
                            {
                                sizeUSINTForBool++;
                                bitAdress = 0;
                                bitAllocation -= 1;
                            }

                            break;

                        case "SINT":

                            sizeUdt++;



                            if (type.Hidden == false)
                            {
                                listVar.Add(new Signal<byte>($"{namePreview}.{type.Name}", "", (uint)byteAdress));
                                byteAdress++;

                            }

                            break;


                        case "USINT":

                            sizeUdt++;



                            if (type.Hidden == false)
                            {
                                listVar.Add(new Signal<sbyte>($"{namePreview}.{type.Name}", "", (uint)byteAdress));
                                byteAdress++;

                            }

                            break;



                        case "UINT":

                            sizeUdt += 2;

                            byteAdress = byteAdress + (byteAdress % 2);
                            if (type.Hidden == false)
                            {
                                listVar.Add(new Signal<ushort>($"{namePreview}.{type.Name}", "", (uint)byteAdress));
                                byteAdress += 2;
                            }
                            break;


                        case "INT":

                            sizeUdt += 2;

                            byteAdress = byteAdress + (byteAdress % 2);
                            if (type.Hidden == false)
                            {
                                listVar.Add(new Signal<short>($"{namePreview}.{type.Name}", "", (uint)byteAdress));
                                byteAdress += 2;
                            }
                            break;



                        case "REAL":

                            sizeUdt += 4;

                            byteAdress = byteAdress + (byteAdress % 4);

                            if (type.Hidden == false)
                            {
                                listVar.Add(new Signal<float>($"{namePreview}.{type.Name}", "", (uint)byteAdress));
                                byteAdress += 4;
                            }

                            break;

                        default:


                            byteAdress = byteAdress + (byteAdress % 4);

                            ResolveUdt(listVar, udt_Manager.listUdt[type.Type], udt_Manager, ref byteAdress, ref bitAdress, $"{namePreview}.{type.Name}{ArrayString}");
                            sizeUdt = 0;
                            break;
                    }


                    typePrecedent = type.Type;

                }

            }

            //Complete la taille de l'udt la derniere variable est un bit
            if (sizeUSINTForBool > 0 & sizeUSINTForBool < 2)
            {
                byteAdress += 2 - sizeUSINTForBool;
                sizeUdt += 1;
                sizeUSINTForBool = 0;
            }

            //Complete la taille de l'udt si celui ci est inférieur à 4 byte et contient des types atomiques
            if (sizeUdt > 0 & sizeUdt < 4)
            {
                byteAdress += 4 - sizeUdt;
            }
            return byteAdress;
        }


    }


    #endregion

}



