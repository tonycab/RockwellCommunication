using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockwellCommunication.IOSignals
{


    public class Signal<T> : Signal
    {

        /// <summary>
        /// Constructor of signal type
        /// </summary>
        /// <param name="name">Name of signal</param>
        /// <param name="designation">Descriptor of signal</param>
        /// <param name="numberByte">byte number adress</param>
        /// <param name="numberBit">bit number adress</param>
        public Signal(string name, string designation, uint numberByte, int numberBit) : base(typeof(T), name, designation, numberByte, numberBit)
        {

        }

        /// <summary>
        /// Constructor of signal type
        /// </summary>
        /// <param name="name">Name of signal</param>
        /// <param name="designation">Descriptor of signal</param>
        /// <param name="numberByte">byte number adress</param>
        public Signal(string name, string designation, uint numberByte) : base(typeof(T), name, designation, numberByte, 0)
        {

        }

    }


    public class Signal : INotifyPropertyChanged
    {
        /// <summary>
        /// Constructor of signal type
        /// </summary>
        /// <param name="type">Type of signal</param>
        /// <param name="name">Name of signal</param>
        /// <param name="designation">Descriptor of signal</param>
        /// <param name="numberByte">byte number adress</param>
        /// <param name="numberBit">bit number adress</param>
        public Signal(Type type, string name, string designation, uint numberByte, int numberBit)
        {
            Name = name;
            Designation = designation;
            ByteAdress = numberByte;
            BitAdress = numberBit;
            Type = type;
            Value = Activator.CreateInstance(type);
        }

        /// <summary>
        /// Constructor of signal type
        /// </summary>
        /// <param name="type">Type of signal</param>
        /// <param name="name">Name of signal</param>
        /// <param name="designation">Descriptor of signal</param>
        /// <param name="numberByte">byte number adress</param>
        public Signal(Type type, string name, string designation, uint numberByte) : this(type, name, designation, numberByte, 0)
        {

        }

        /// <summary>
        /// Type of signal after call constructor
        /// </summary>
        public Type Type { get; private set; }


        /// <summary>
        /// Event property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        private string name;

        /// <summary>
        /// Name of signam
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value != name)
                {
                    name = value;
                    onPropertyChanged(nameof(Name));
                }
            }

        }

        private string designation;

        /// <summary>
        /// Descriptor signal
        /// </summary>
        public string Designation
        {
            get
            {
                return designation;
            }
            set
            {
                if (value != designation)
                {
                    designation = value;
                    onPropertyChanged(nameof(Designation));
                }
            }

        }

        /// <summary>
        /// Number byte adress
        /// </summary>
        public uint ByteAdress { get; set; }

        /// <summary>
        /// Number bit adress
        /// </summary>
        public int BitAdress { get; set; }


        private dynamic value;

        /// <summary>
        /// Value of signal (Dynamic)
        /// </summary>
        public dynamic Value
        {
            get
            {
                return value;
            }
            set
            {
                if (!value.Equals(this.value))
                {


                    if (value.GetType() != Type)
                    {
                        this.value =Convert.ChangeType(value, Type);
                    }
                    else { 

                    this.value = value;

                    }

                    onPropertyChanged(nameof(Value));

                    stateChanged = true;
                }
            }

        }


      




        /// <summary>
        /// Call event propertie change
        /// </summary>
        /// <param name="propertyName"></param>
        public void onPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(propertyName)));



        private bool stateChanged { get; set; }

        /// <summary>
        /// Event value has changed
        /// </summary>
        public event Action<Signal> SignalChanged;


        /// <summary>
        /// Call event signal changed
        /// </summary>
        public void CallEventStateChanged()
        {
            if (stateChanged == true)
            {
                this.SignalChanged?.Invoke(this);
                stateChanged = false;
            }
        }



        /// <summary>
        /// Get the value in to byte array
        /// </summary>
        /// <param name="register"></param>
        public void SetValue(byte[] register)
        {

            if (Value.GetType() == typeof(bool))
            {
                var v = (register[ByteAdress] >> BitAdress & 1);

                Value = v == 1 ? true : false;
            }

            if (Value.GetType() == typeof(sbyte))
            {
                Value = (sbyte)register[BitAdress];
            }

            if (Value.GetType() == typeof(byte))
            {
                Value = (byte)register[BitAdress];
            }

            if (Value.GetType() == typeof(ushort))
            {
                var v = BitConverter.ToUInt16(register, (int)ByteAdress);
                Value = v;
            }

            if (Value.GetType() == typeof(short))
            {
                var v = BitConverter.ToInt16(register, (int)ByteAdress);
                Value = v;
            }

            if (Value.GetType() == typeof(int))
            {
                var v = BitConverter.ToInt32(register, (int)ByteAdress);
                Value = v;
            }

            if (Value.GetType() == typeof(uint))
            {
                var v = BitConverter.ToUInt32(register, (int)ByteAdress);
                Value = v;
            }


            if (Value.GetType() == typeof(float))
            {
                var v = BitConverter.ToSingle(register, (int)ByteAdress);
                Value = v;
            }

        }

        /// <summary>
        /// Set the value in to byte arrat
        /// </summary>
        /// <param name="register"></param>
        public void SetRegister(byte[] register)
        {

            //Convert string to value type constructor
            if (Value.GetType() == typeof(string))
            {

                if (Type == typeof(bool))
                {
                    Value = bool.Parse(Value);
                }
                if (Type == typeof(sbyte))
                {
                    Value = sbyte.Parse(Value);
                }
                if (Type == typeof(byte))
                {
                    Value = byte.Parse(Value);
                }
                if (Type == typeof(ushort))
                {
                    Value = ushort.Parse(Value);
                }
                if (Type == typeof(short))
                {
                    Value = short.Parse(Value);
                }
                if (Type == typeof(float))
                {

                    Value = float.Parse(Value.Replace('.', ','));
                }
            }

            //Set byte array with bool type
            if (Value.GetType() == typeof(bool))
            {
                byte v;

                if ((bool)(object)(Value) == true)
                {
                    v = (byte)(1 << BitAdress);
                    register[ByteAdress] = (byte)(v | register[ByteAdress]);
                }
                else
                {
                    v = (byte)~(1 << BitAdress);

                    register[ByteAdress] = (byte)(v & register[ByteAdress]);
                }

            }
            //Set byte array with sbyte type
            if (Value.GetType() == typeof(sbyte))
            {
                register[ByteAdress] = (byte)(Value);
            }
            //Set byte array with byte type
            if (Value.GetType() == typeof(byte))
            {
                register[ByteAdress] = (byte)(Value);
            }
            //Set byte array with short type
            if (Value.GetType() == typeof(short))
            {
                register[ByteAdress] = Convert.ToByte(Value);
                register[ByteAdress + 1] = Convert.ToByte((Convert.ToInt16(Value) >> 8));
            }
            //Set byte array with ushort type
            if (Value.GetType() == typeof(ushort))
            {
                register[ByteAdress] = (byte)(Value);
                register[ByteAdress + 1] = (byte)((Convert.ToInt16(Value) >> 8));
            }

            //Set byte array with int type
            if (Value.GetType() == typeof(int))
            {

                register[ByteAdress] = (byte)(Value);
                register[ByteAdress + 1] = (byte)((Convert.ToInt16(Value) >> 8));
                register[ByteAdress + 2] = (byte)((Convert.ToInt16(Value) >> 16));
                register[ByteAdress + 3] = (byte)((Convert.ToInt16(Value) >> 24));
            }
            //Set byte array with uint type
            if (Value.GetType() == typeof(uint))
            {

                register[ByteAdress] = Convert.ToByte(Value);
                register[ByteAdress + 1] = Convert.ToByte((Convert.ToInt16(Value) >> 8));
                register[ByteAdress + 2] = Convert.ToByte((Convert.ToInt16(Value) >> 16));
                register[ByteAdress + 3] = Convert.ToByte((Convert.ToInt16(Value) >> 24));
            }
            //Set byte array with float type
            if (Value.GetType() == typeof(float))
            {
                float valueFloat = (float)(object)Value;

                byte[] v = BitConverter.GetBytes(valueFloat);

                register[ByteAdress] = v[0];
                register[ByteAdress + 1] = v[1];
                register[ByteAdress + 2] = v[2];
                register[ByteAdress + 3] = v[3];
            }
        }
    }
}
