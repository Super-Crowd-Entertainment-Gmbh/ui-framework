using System;

namespace Rehawk.UIFramework
{
    public class MemberBindingStrategy : IBindingStrategy
    {
        private readonly BindedMember member;
        
        public event EventHandler GotDirty;

        public MemberBindingStrategy(Func<object> getOriginFunction, string memberPath)
        {
            member = new BindedMember(getOriginFunction, memberPath);
            member.GotDirty += OnMemberGotDirty;
        }
        
        public void Evaluate()
        {
            member.Evaluate();
        }
        
        public void Release()
        {
            member.Release();
        }

        public object Get()
        {
            return member.Get();
        }

        public void Set(object value)
        {
            member.Set(value);
        }

        private void OnMemberGotDirty()
        {
            GotDirty?.Invoke(this, EventArgs.Empty);
        }
    }
}