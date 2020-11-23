using System.Windows.Input;
using Xamarin.Forms;
using XFWeatherApp.Models;

namespace XFWeatherApp.Behaviors
{
    public class TextChangedBehavior : Behavior<Entry>
    {
        public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(TextChangedBehavior), null);
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        protected override void OnAttachedTo(Entry bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.TextChanged += Bindable_TextChanged;
        }
        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= Bindable_TextChanged;
            base.OnDetachingFrom(bindable);
        }
        void Bindable_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Command == null) return;

            if (Command.CanExecute(e.NewTextValue))
            {
                Command.Execute(e.NewTextValue);
            }
        }
    }
}
