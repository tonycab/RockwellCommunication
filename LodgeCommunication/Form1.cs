using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RockwellCommunication;
using RockwellCommunication.IOSignals;
using RockwellCommunication.Protocol;



namespace LodgeCommunication
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        LodgeEthernetIP eip;

        private void Form1_Load(object sender, EventArgs e)
        {

            //Simulation envois de données aléatoirement cohérente
            Random random = new Random(10);

            //Log démarrage application
            LogsManager.Add(EnumCategory.Info, "Application", "Demarrage application");

            //Initialisation datagridvew3 affichage des logs
            dataGridView3.DataSource = LogsManager.GetLogs();
            dataGridView3.Columns[0].Width = 200;
            dataGridView3.Columns[1].Width = 150;
            dataGridView3.Columns[2].Width = 150;
            dataGridView3.RowsAdded += MiseEnFormeLogs;
            

            //Rémanence des logs dans le datagridvieuw
            dataGridView3.RowsAdded += (o, s) =>
            {
                dataGridView3.FirstDisplayedScrollingRowIndex = dataGridView3.RowCount - 1;
            };

            //Intanciation d'une communication Modbus avec le PLC
            eip = new LodgeEthernetIP();

            eip.EndReadWriteEnd += UpdateReadWrite;
            eip.LogError += (Error) => this.Invoke((Action)(()=> LogsManager.Add(EnumCategory.Error, "Interne error ", Error)));

            //Abonnement évennement changement d'état de la connection 
            eip.EthernetIPStateEventArgs += StateCommunication;

            //Mise en forme des Rows du datagridvieuw1 et 2 Affichage des signaux
            dataGridView1.RowsAdded += MiseEnForme;
            dataGridView2.RowsAdded += MiseEnForme;
            dataGridView2.CellContentDoubleClick += ClickCell;
            dataGridView2.CellValidating += dataGridView2_CellValidating;

            //Databinding des signaux entrées et sorties Modbus
            dataGridView1.DataSource = eip.Inputs;
            dataGridView2.DataSource = eip.Outputs;

            //Affiche le numero de table devant la caméra 2
            eip.NumberTableChanged += (o)=>this.Invoke((Action)(() => textBox3.Text = o.ToString()));

            //Détection d'une demande de trigg du PLC
            eip.EvtTrigger3DTable += (s1, s2, s3, s4) =>
            {

                //Trigg pour la caméra 1
                if (s4[0] == "Cam1")
                {
                    //Thead camera 1
                    Task t = Task.Run(() =>
                    {

                        //Acquistion de l'image
                        this.Invoke(new Action(() => { LogsManager.Add(EnumCategory.Process, "Camera 1", "Demande de trigger camera 1 "); }));

                        //Temps de simulation d'acquisition d'image
                        Thread.Sleep(2000);

                        this.Invoke(new Action(() => { LogsManager.Add(EnumCategory.Process, "Camera 1", "Trigger terminér camera 1"); }));
                        //Fin d'acquisition de l'image

                        //Traitement de l'image dans un autre Thread parallèle
                        Task.Run(() =>
                        {

                            this.Invoke(new Action(() => { LogsManager.Add(EnumCategory.Process, "Camera 1", "Traitement en cours camera 1"); }));

                            Thread.Sleep(2000);

                            this.Invoke(new Action(() => { LogsManager.Add(EnumCategory.Process, "Camera 1", "Données prêtes camera 1"); }));



                            string state = random.RandonRangeInt(0, 5) > 1 ? "PASS" : "FAIL";

                            //NumberFormatInfo.CurrentInfo.NumberDecimalSeparator = ".";
                            double score = 0;
                            double x = 0;
                            double y = 0;
                            double z = 0;
                            double rx = 0;
                            double ry = 0;
                            double rz = 0;

                            if (state == "PASS")
                            {
                                score = random.RandonRangeDouble(30, 90);
                                x = random.RandonRangeDouble(-150, 150);
                                y = random.RandonRangeDouble(-150, 150);
                                z = random.RandonRangeDouble(-10, 10);
                                rx = random.RandonRangeDouble(-10, 10);
                                ry = random.RandonRangeDouble(-10, 10);
                                rz = random.RandonRangeDouble(-10, 10);
                            }

                            string ExtensionAttaque = "";

                            var numberAttaque = random.RandonRangeInt(0, 20);
                            ExtensionAttaque += numberAttaque + ";";
                            if (numberAttaque > 0)
                            {

                                for (int i = 0; i < numberAttaque; i++)
                                {

                                    ExtensionAttaque += random.RandonRangeDouble(-150, 150) + ";";
                                    ExtensionAttaque += random.RandonRangeDouble(-150, 150) + ";";
                                    ExtensionAttaque += random.RandonRangeDouble(-150, 150) + ";";

                                }
                            }

                            if (s1 == "0") s1 = "1";
                            eip.SendMessage($"/trigger;{s1};{s2};{s3};{state};{score};{x};{y};{z};{rx};{ry};{rz};{ExtensionAttaque}");

                        });
                    });

                    //Attente fin d'acquistion d'image
                    t.Wait();
                }


                //Trigg pour la caméra 2
                if (s4[0] == "Cam2")
                {

                    //Thead camera 2
                    Task t = Task.Run(() =>
                    {

                        //Acquistion de l'image
                        this.Invoke(new Action(() => { LogsManager.Add(EnumCategory.Process, "Camera 2", "Demande de trigger camera 2"); }));

                        //Temps de simulation d'acquisition d'image
                        Thread.Sleep(2500);

                        this.Invoke(new Action(() => { LogsManager.Add(EnumCategory.Process, "Camera 2", "Trigger terminér camera 2"); }));
                        //Fin d'acquisition de l'image


                        //Traitement de l'image dans un autre Thread parallèle
                        Task.Run(() =>
                        {

                            this.Invoke(new Action(() => { LogsManager.Add(EnumCategory.Process, "Camera 2", "Traitement en cours camera 2"); }));

                            Thread.Sleep(2000);

                            this.Invoke(new Action(() => { LogsManager.Add(EnumCategory.Process, "Camera 2", "Données prêtes camera 2"); }));



                            ////Simulation envois de données aléatoirement cohérente
                            //Random random = new Random();

                            string state = random.RandonRangeInt(0, 5) > 1 ? "PASS" : "FAIL";

                            //NumberFormatInfo.CurrentInfo.NumberDecimalSeparator = ".";
                            double score = 0;
                            double x = 0;
                            double y = 0;
                            double z = 0;
                            double rx = 0;
                            double ry = 0;
                            double rz = 0;

                            if (state == "PASS")
                            {
                                score = random.RandonRangeDouble(30, 90);
                                x = random.RandonRangeDouble(-150, 150);
                                y = random.RandonRangeDouble(-150, 150);
                                z = random.RandonRangeDouble(-10, 10);
                                rx = random.RandonRangeDouble(-10, 10);
                                ry = random.RandonRangeDouble(-10, 10);
                                rz = random.RandonRangeDouble(-10, 10);
                            }

                            string ExtensionAttaque = "";

                            var numberAttaque = random.RandonRangeInt(0, 20);
                            ExtensionAttaque += numberAttaque + ";";
                            if (numberAttaque > 0)
                            {

                                for (int i = 0; i < numberAttaque; i++)
                                {

                                    ExtensionAttaque += random.RandonRangeDouble(-150, 150) + ";";
                                    ExtensionAttaque += random.RandonRangeDouble(-150, 150) + ";";
                                    ExtensionAttaque += random.RandonRangeDouble(-150, 150) + ";";

                                }
                            }



                            if (s1 == "0") s1 = "20001";
                            eip.SendMessage($"/trigger;{s1};{s2};{s3};{state};{score};{x};{y};{z};{rx};{ry};{rz};{ExtensionAttaque}");

                        });
                    });

                    //Attente fin d'acquistion d'image
                    t.Wait();
                }
            };

        }

        //Affiche le temps de cycle d echange EIP
        private void UpdateReadWrite(long obj)
        {
            this.Invoke(new Action(() => {
                textBox2.Text = obj.ToString();
            }));
        }


        //Etat de la connection 
        private void StateCommunication(object sender, EthernetIPEventArgs e)
        {

            this.Invoke(new Action(() => LogsManager.Add(EnumCategory.Communication, "Socket", e)));

            if (e.State == StateConnect.Connecting)
            {
                this.Invoke(new Action(() => { button3.BackColor = Color.LightGreen; }));
                this.Invoke(new Action(() => { button2.BackColor = SystemColors.Control; }));
            }

            if (e.State == StateConnect.Connected)
            {
                this.Invoke(new Action(() => { button3.BackColor = Color.Green; }));
                this.Invoke(new Action(() => { button2.BackColor = SystemColors.Control; }));
            }

            if (e.State == StateConnect.NoClientExist)
            {
                this.Invoke(new Action(() => { button3.BackColor = Color.Yellow; }));
                this.Invoke(new Action(() => { button2.BackColor = SystemColors.Control; }));
            }

            if (e.State == StateConnect.Stopped)
            {
                this.Invoke(new Action(() => { button3.BackColor = SystemColors.Control; }));
                this.Invoke(new Action(() => { button2.BackColor = SystemColors.Control; }));
            }

            if (e.State == StateConnect.Disconnected)
            {
                this.Invoke(new Action(() => { button3.BackColor = SystemColors.Control; }));
                this.Invoke(new Action(() => { button2.BackColor = Color.Red; }));
            }
        }

        //MEthode de mise en forme conditionnelle du type de signaux
        private void MiseEnForme(object sender, DataGridViewRowsAddedEventArgs e)
        {
            DataGridView d = (DataGridView)sender;

            for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
            {
                Signal v = (Signal)d.Rows[i].DataBoundItem;


                d.Rows[i].Cells["Type"].ReadOnly = true;
                d.Rows[i].Cells["Name"].ReadOnly = true;
                d.Rows[i].Cells["Designation"].ReadOnly = true;
                d.Rows[i].Cells["ByteAdress"].ReadOnly = true;
                d.Rows[i].Cells["BitAdress"].ReadOnly = true;

                if (v.GetType() == typeof(Signal<bool>))
                {
                    d.Rows[i].DefaultCellStyle.BackColor = Color.MediumSeaGreen;
                    d.Rows[i].Cells["Value"].ReadOnly = true;
                }
                else if (v.GetType() == typeof(Signal<short>))
                {
                    d.Rows[i].DefaultCellStyle.BackColor = Color.LavenderBlush;
                    d.Rows[i].Cells["Value"].ReadOnly = false;
                }
                else if (v.GetType() == typeof(Signal<ushort>))
                {
                    d.Rows[i].DefaultCellStyle.BackColor = Color.Lavender;
                    d.Rows[i].Cells["Value"].ReadOnly = false;
                }
                else if (v.GetType() == typeof(Signal<int>))
                {
                    d.Rows[i].DefaultCellStyle.BackColor = Color.Olive;
                    d.Rows[i].Cells["Value"].ReadOnly = false;
                }
                else if (v.GetType() == typeof(Signal<uint>))
                {
                    d.Rows[i].DefaultCellStyle.BackColor = Color.OliveDrab;
                    d.Rows[i].Cells["Value"].ReadOnly = false;
                }
                else if (v.GetType() == typeof(Signal<float>))
                {
                    d.Rows[i].DefaultCellStyle.BackColor = Color.GreenYellow;
                    d.Rows[i].Cells["Value"].ReadOnly = false;
                }
                else
                {
                    d.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                    d.Rows[i].Cells["Value"].ReadOnly = false;
                }
            }
        }

        private void dataGridView2_CellValidating(object sender,
            DataGridViewCellValidatingEventArgs e)
        {
            DataGridView d = (DataGridView)sender;

            Signal v = (Signal)d.Rows[e.RowIndex].DataBoundItem;

            d.Rows[e.RowIndex].Cells["Value"].ErrorText = "";

            if (d.SelectedCells[0].ColumnIndex!= d.Columns["Value"].Index) return;

            sbyte valsbyte;
            if (v.Type == typeof(sbyte))
            {
                if (!sbyte.TryParse(e.FormattedValue.ToString(),
                    out valsbyte))
                {
                    e.Cancel = true;
                    d.Rows[e.RowIndex].Cells["Value"].ErrorText = "Error value is not byte 8bit signed)";
                }
            }

            byte valbyte;
            if (v.Type == typeof(byte))
            {
                if (!byte.TryParse(e.FormattedValue.ToString(),
                    out valbyte))
                {
                    e.Cancel = true;
                    d.Rows[e.RowIndex].Cells["Value"].ErrorText = "Error value is not byte 8bit unsigned)";
                }
            }

            ushort valushort;
            if (v.Type == typeof(ushort))
            {
                if (!ushort.TryParse(e.FormattedValue.ToString(),
                    out valushort))
                {
                    e.Cancel = true;
                    d.Rows[e.RowIndex].Cells["Value"].ErrorText = "Error value is not ushort 16bit unsigned)";
                }
            }

            short valshort;
            if (v.Type == typeof(short))
            {
                if (!short.TryParse(e.FormattedValue.ToString(),
                    out valshort))
                {
                    e.Cancel = true;
                    d.Rows[e.RowIndex].Cells["Value"].ErrorText = "Error value is not short 16bit signed)";
                }
            }

            uint valuint;
            if (v.Type == typeof(uint))
            {
                if (!uint.TryParse(e.FormattedValue.ToString(),
                    out valuint))
                {
                    e.Cancel = true;
                    d.Rows[e.RowIndex].Cells["Value"].ErrorText = "Error value is not int 16bit unsigned)";
                }
            }

            int valint;
            if (v.Type == typeof(int))
            {
                if (!int.TryParse(e.FormattedValue.ToString(),
                    out valint))
                {
                    e.Cancel = true;
                    d.Rows[e.RowIndex].Cells["Value"].ErrorText = "Error value is not int 32bit signed)";
                }
            }


            float valfloat;
            if (v.Type == typeof(float))
            {
                if (!float.TryParse(e.FormattedValue.ToString(),
                    out valfloat))
                {
                    e.Cancel = true;
                    d.Rows[e.RowIndex].Cells["Value"].ErrorText = "Error value is not float 32bit)";
                }
            }




        }


        private void ClickCell(object sender, DataGridViewCellEventArgs e)
        {

            DataGridView d = (DataGridView)sender;

            Signal v = (Signal)d.Rows[e.RowIndex].DataBoundItem;

            if (e.ColumnIndex != d.Columns["Value"].Index) return;
            
            if (v.Type == typeof(bool))
            {

                v.Value = !v.Value;

            }


        }


        //Mise en forme conditionnelle des types de logs
        private void MiseEnFormeLogs(object sender, DataGridViewRowsAddedEventArgs e)
        {
            DataGridView d = (DataGridView)sender;

            for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
            {
                Log v = (Log)d.Rows[i].DataBoundItem;

                if (v.Type == "Camera 1")
                {
                    d.Rows[i].DefaultCellStyle.BackColor = Color.MediumSeaGreen;
                }
                else if (v.Type == "Camera 2")
                {
                    d.Rows[i].DefaultCellStyle.BackColor = Color.LightGreen;
                }

                if(v.Category == EnumCategory.Error) {

                    d.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;
                }

            }
        }


        //Effacement des logs
        private void button1_Click_1(object sender, EventArgs e)
        {
            LogsManager.Clear();
        }

        //Lancement de la communication modbus
        private void button3_Click(object sender, EventArgs e)
        {
            eip.IP = textBox1.Text;
            eip?.Start();
        }

        //Arret de la communcation modbus
        private void button2_Click(object sender, EventArgs e)
        {
            eip?.Stop();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            eip.FaultCam1 = eip.FaultCam1==true  ? false : true;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            eip.FaultCam2 = eip.FaultCam2 == true ? false : true;
        }

   
    }





}

