using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockwellCommunication.Protocol
{
    public class EthernetIPEventArgs
    {
        #region Properties public

        public StateConnect State;
        public string IP;
        #endregion

        #region Constuctor 
        /// <summary>
        /// Constructor class informations event state conection Modbus
        /// </summary>
        /// <param name="stateConnect"></param>
        /// <param name="adresse"></param>
        /// <param name="port"></param>
        public EthernetIPEventArgs(StateConnect stateConnect, string adresse)
        {

            State = stateConnect;
            IP = adresse;
        }

        #endregion

        #region Methode public

        /// <summary>
        /// Return string of state information 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Enum.GetName(typeof(StateConnect), State) + " to IP: " + IP ;
        }

        /// <summary>
        /// Convertion implicite ToString() Methode
        /// </summary>
        /// <param name="s"></param>
        public static implicit operator string(EthernetIPEventArgs s) => s.ToString();

        #endregion
    }
}
