using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Rehawk.UIFramework
{
    public abstract class UIControlBase : UIBehaviour, INotifyPropertyChanged
    {
        private UIPanel parentPanel;
        
        private readonly Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();
        private readonly List<Binding> bindings = new List<Binding>();

        public event PropertyChangedEventHandler PropertyChanged;

        public UIPanel ParentPanel
        {
            get { return parentPanel; }
        }

        protected override void Awake()
        {
            base.Awake();
            
            parentPanel = GetComponentInParent<UIPanel>();

            if (parentPanel)
            {
                parentPanel.BecameVisible += OnPanelBecameVisible;
                parentPanel.BecameInvisible += OnPanelBecameInvisible;
            }
        }

        protected override void Start()
        {
            base.Start();
            
            SetupBindingsInternal();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (parentPanel)
            {
                parentPanel.BecameVisible -= OnPanelBecameVisible;
                parentPanel.BecameInvisible -= OnPanelBecameInvisible;
            }

            ReleaseBindings();
        }
        
        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();

            if (parentPanel)
            {
                parentPanel.BecameVisible -= OnPanelBecameVisible;
                parentPanel.BecameInvisible -= OnPanelBecameInvisible;
            }
            
            parentPanel = GetComponentInParent<UIPanel>();
            
            if (parentPanel)
            {
                parentPanel.BecameVisible += OnPanelBecameVisible;
                parentPanel.BecameInvisible += OnPanelBecameInvisible;
            }
        }
        
        private void SetupBindingsInternal()
        {
            SetupBindings();
            EvaluateBindings();
        }
        
        protected virtual void SetupBindings() { }
        
        public void SetCommand(string commandName, ICommand command)
        {
            commands[commandName] = command;
        }

        public void InvokeCommand<T>(string commandName, T args) where T : ICommandArgs
        {
            if (commands.TryGetValue(commandName, out ICommand command))
            {
                command.Execute(this, args);
            }
        }

        public void InvokeCommand(string commandName)
        {
            InvokeCommand(commandName, CommandArgs.Empty);
        }

        public Binding Bind<T>(Expression<Func<T>> memberExpression, BindingDirection direction = BindingDirection.OneWay)
        {
            var binding = Binding.Bind(this, () => this, memberExpression, direction);
            
            bindings.Add(binding);
            
            return binding;
        }

        /// <summary>
        /// Creates a new binding which will be reevaluated each time this node gets dirty.
        /// </summary>
        /// <param name="getContext">Should return the context object. If it's type implements <see cref="System.ComponentModel.INotifyPropertyChanged"/> or <see cref="System.Collections.Specialized.INotifyCollectionChanged"/> it will react to changes without setting the whole node dirty.</param>
        /// <param name="propertyName">Can be provided to distinguish context and data source from each other. Helpful in cases where the data source doesn't implements <see cref="System.ComponentModel.INotifyPropertyChanged"/> or <see cref="System.Collections.Specialized.INotifyCollectionChanged"/>. If it's null or empty, the context is the data source.</param>
        public Binding BindProperty(Func<object> getContext, string propertyName, BindingDirection direction = BindingDirection.OneWay)
        {
            var binding = Binding.BindProperty(this, getContext, propertyName, direction);
            
            bindings.Add(binding);
            
            return binding;
        }

        public Binding BindContext(Func<UIContextControlBase> getControlCallback, BindingDirection direction = BindingDirection.OneWay)
        {
            var binding = Binding.BindContext(this, getControlCallback, direction);
            
            bindings.Add(binding);
            
            return binding;
        }
        
        public Binding BindCallback<T>(Action<T> setCallback)
        {
            var binding = Binding.BindCallback<T>(this, () => default, setCallback);
            
            bindings.Add(binding);
            
            return binding;
        }
        
        public Binding BindCallback<T>(Func<T> getCallback, Action<T> setCallback)
        {
            var binding = Binding.BindCallback<T>(this, getCallback, setCallback);
            
            bindings.Add(binding);
            
            return binding;
        }

        [ContextMenu("Evaluate Bindings")]
        protected void EvaluateBindings()
        {
            for (int i = 0; i < bindings.Count; i++)
            {
                bindings[i].Evaluate();
                bindings[i].SourceToDestination();
            }
        }

        private void ReleaseBindings()
        {
            for (int i = 0; i < bindings.Count; i++)
            {
                bindings[i].Release();
            }
        }

        public virtual void SetDirty()
        {
            OnRefresh();
        }

        protected virtual void OnRefresh() {}
        
        protected virtual void OnPanelBecameVisible() {}
        protected virtual void OnPanelBecameInvisible() {}
        
        private void OnPanelBecameVisible(object sender, UIPanel panel)
        {
            SetDirty();
            OnPanelBecameVisible();
        }

        private void OnPanelBecameInvisible(object sender, UIPanel panel)
        {
            OnPanelBecameInvisible();
        }
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);
            
            return true;
        }
    }
}