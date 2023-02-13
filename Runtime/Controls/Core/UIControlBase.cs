using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine.EventSystems;

namespace Rehawk.UIFramework
{
    public abstract class UIControlBase : UIBehaviour, INotifyPropertyChanged
    {
        private readonly Dictionary<string, CommandBase> commands = new Dictionary<string, CommandBase>();
        private readonly List<Binding> bindings = new List<Binding>();

        public event PropertyChangedEventHandler PropertyChanged;
        
        public void SetCommand(string commandName, CommandBase command)
        {
            commands[commandName] = command;
        }

        public void InvokeCommand<T>(string commandName, T args) where T : ICommandArgs
        {
            if (commands.TryGetValue(commandName, out CommandBase command))
            {
                command.Execute(this, args);
            }
        }

        public void InvokeCommand(string commandName)
        {
            InvokeCommand(commandName, ICommandArgs.Empty);
        }

        /// <summary>
        /// Creates a new binding which will be reevaluated each time this node gets dirty.
        /// </summary>
        /// <param name="getContext">Should return the context object. If it's type implements <see cref="System.ComponentModel.INotifyPropertyChanged"/> or <see cref="System.Collections.Specialized.INotifyCollectionChanged"/> it will react to changes without setting the whole node dirty.</param>
        /// <param name="propertyName">Can be provided to distinguish context and data source from each other. Helpful in cases where the data source doesn't implements <see cref="System.ComponentModel.INotifyPropertyChanged"/> or <see cref="System.Collections.Specialized.INotifyCollectionChanged"/>. If it's null or empty, the context is the data source.</param>
        public Binding Bind(Func<object> getContext, string propertyName = null, BindingDirection direction = BindingDirection.OneWay)
        {
            var binding = Binding.Bind(getContext, propertyName, direction);
            
            bindings.Add(binding);
            
            return binding;
        }

        public void EvaluateBindings()
        {
            for (int i = 0; i < bindings.Count; i++)
            {
                bindings[i].Evaluate();
                bindings[i].OriginToDestination();
            }
        }

        public virtual void SetDirty()
        {
            OnRefresh();
        }

        protected virtual void OnRefresh() {}
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}