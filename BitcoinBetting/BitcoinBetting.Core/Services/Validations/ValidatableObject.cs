using BitcoinBetting.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BitcoinBetting.Core.Services.Validations
{
    public class ValidatableObject<T> : BindableObject, IValidity
    {
        private readonly List<IValidationRule<T>> _validations;
        private List<string> _errors;
        private T _value;
        private bool _isValid;

        public List<IValidationRule<T>> Validations => _validations;

        public List<string> Errors
        {
            get
            {
                return _errors;
            }
            set
            {
                _errors = value;
                RaisePropertyChanged(() => Errors);
            }
        }

        public T Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                RaisePropertyChanged(() => Value);
            }
        }

        public bool IsValid
        {
            get
            {
                return _isValid;
            }
            set
            {
                _isValid = value;
                RaisePropertyChanged(() => IsValid);
            }
        }

        public ValidatableObject()
        {
            _isValid = true;
            _errors = new List<string>();
            _validations = new List<IValidationRule<T>>();
        }

        public bool Validate()
        {
            Errors.Clear();

            Errors = _validations
                .Where(v => !v.Check(Value))
                .Select(v => v.ValidationMessage)
                .ToList();
            
            IsValid = !Errors.Any();

            return this.IsValid;
        }

        public void RaisePropertyChanged<T>(Expression<Func<T>> property)
        {
            var name = GetMemberInfo(property).Name;
            OnPropertyChanged(name);
        }

        private MemberInfo GetMemberInfo(Expression expression)
        {
            MemberExpression operand;
            LambdaExpression lambdaExpression;
            
            try
            {
                lambdaExpression = (LambdaExpression)expression;
                
                if (lambdaExpression.Body is UnaryExpression)
                {
                    UnaryExpression body = (UnaryExpression)lambdaExpression.Body;
                    operand = (MemberExpression)body.Operand;
                }
                else
                {
                    operand = (MemberExpression)lambdaExpression.Body;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
                throw;
            }
            
            return operand.Member;
        }
    }
}
