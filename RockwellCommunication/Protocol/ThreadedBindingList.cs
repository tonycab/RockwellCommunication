using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RockwellCommunication.Protocol
{
    /// <summary>
    /// Class List<T> ThreadSafe, Les méthodes ADD, ListChange sont exécuter toujours dans le thread d'instanciation de la class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ThreadedBindingList<T> : BindingList<T>
    {

        SynchronizationContext ctxOrigin = SynchronizationContext.Current;

        protected override void OnAddingNew(AddingNewEventArgs e)
        {
            SynchronizationContext ctx = SynchronizationContext.Current;
            if (ctx == ctxOrigin)
            {
                BaseAddingNew(e);
            }
            else
            {
                ctxOrigin.Send(delegate
                {
                    BaseAddingNew(e);
                }, null);
            }
        }
        void BaseAddingNew(AddingNewEventArgs e)
        {
            base.OnAddingNew(e);
        }
        protected override void OnListChanged(ListChangedEventArgs e)
        {
            SynchronizationContext ctx = SynchronizationContext.Current;
            if (ctx == ctxOrigin)
            {
                BaseListChanged(e);
            }
            else
            {
                ctxOrigin.Send(delegate
                {
                    BaseListChanged(e);
                }, null);
            }
        }
        void BaseListChanged(ListChangedEventArgs e)
        {
            SynchronizationContext ctx = SynchronizationContext.Current;
            if (ctx == ctxOrigin)
            {
                base.OnListChanged(e);
            }
            else
            {
                ctxOrigin.Send(delegate
                {
                    base.OnListChanged(e);
                }, null);
            }
        }
    }
}
