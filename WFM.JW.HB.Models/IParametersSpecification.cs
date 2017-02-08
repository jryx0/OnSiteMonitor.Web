using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WFM.JW.HB.Models
{
    //public struct DBValue
    //{
    //    string Name;
    //    Object Value;
    //    System.Data.DbType dbType;
    //}

    public interface IParametersSpecification
    {
        IParametersSpecification And(IParametersSpecification other);
        IParametersSpecification Or(IParametersSpecification other);

        HashSet<Object> GetParamValues();
        string GetSpec();
        string GetSpecValue();
    }

    public abstract class CompositeSpecification : IParametersSpecification
    {     
        protected static HashSet<Object> _paravalues = new HashSet<object>();
        public abstract string GetSpec();
        public abstract string GetSpecValue();

        public IParametersSpecification And(IParametersSpecification other)
        {
            return new AddSpecification(this, other);
        }

        public IParametersSpecification Or(IParametersSpecification other)
        {
            return new OrSpecification(this, other);
        }

        public HashSet<object> GetParamValues()
        {
            return _paravalues;           
        }
    }

    public class AddSpecification : CompositeSpecification
    {
        private IParametersSpecification _leftQuerySpecification;
        private IParametersSpecification _rightQuerySpecification;
        

        public AddSpecification(IParametersSpecification leftQuerySpecification, IParametersSpecification rightQuerySpecification)
        {
            _leftQuerySpecification = leftQuerySpecification;
            _rightQuerySpecification = rightQuerySpecification;

        }

        public override string GetSpec()
        {
            return " " + _leftQuerySpecification.GetSpec() + " AND " + _rightQuerySpecification.GetSpec() + " ";
        }

        public override string GetSpecValue()
        {
            return "(" + _leftQuerySpecification.GetSpecValue() + " AND " + _rightQuerySpecification.GetSpecValue() + ")";
        }
    }

    public class OrSpecification : CompositeSpecification
    {
        private IParametersSpecification _leftQuerySpecification;
        private IParametersSpecification _rightQuerySpecification;

        public OrSpecification(IParametersSpecification leftQuerySpecification, IParametersSpecification rightQuerySpecification)
        {
            _leftQuerySpecification = leftQuerySpecification;
            _rightQuerySpecification = rightQuerySpecification;

           // Parameters += leftQuerySpecification.Parameters + " OR " + rightQuerySpecification.Parameters;
        }

        public override string GetSpec()
        {
            return "(" + _leftQuerySpecification.GetSpec() + " OR " + _rightQuerySpecification.GetSpec() + ")";
        }

        public override string GetSpecValue()
        {
            return "(" + _leftQuerySpecification.GetSpecValue() + " OR " + _rightQuerySpecification.GetSpecValue() + ")";
        }
    }


    public enum CriteriaOperator
    {
        Less = 0,               // <  
        LessThanOrEqual,    // <=
        Equal,              // =
        NotEqual,           //!=
        GreaterThanOrEqual, // >=
        Greater,            //>
        Like,               // %%
        In,                 // in
        Between,            // Between
        NotLike,
        NotApplicable
        // TODO: Implement remainder of the criteria operators...
    }

    public class CriteriaSpecification : CompositeSpecification
    {
        private string _propertyName;
        private CriteriaOperator _criteriaOperator;

        Object _value;

        /**
         * 参数化查询        
         */
        public CriteriaSpecification(string propertyName, CriteriaOperator criteriaOperator, Object value)
        {
            _propertyName = propertyName;
            _criteriaOperator = criteriaOperator;
            //_value = value;
            _value = value;

            _paravalues.Add(value);            
        }

        /**
         * 空参数调用，意味着无查询条件* 
         */
        public CriteriaSpecification()
        {
            _propertyName = null;
        }

        

        public override string GetSpec()
        {
            if (String.IsNullOrEmpty(_propertyName))
                return "";

            string criteria = "";
            switch (_criteriaOperator)
            {
                case CriteriaOperator.Less:
                    criteria = _propertyName + " < @" + _propertyName;
                    break;
                case CriteriaOperator.LessThanOrEqual:    // <=
                    criteria = _propertyName + " <= @" + _propertyName;
                    break;
                case CriteriaOperator.Equal:              // =
                    criteria = _propertyName + " = @" + _propertyName;
                    break;
                case CriteriaOperator.NotEqual:           //!=
                    criteria = _propertyName + " != @" + _propertyName;
                    break;
                case CriteriaOperator.GreaterThanOrEqual: // >=
                    criteria = _propertyName + " >= @" + _propertyName;
                    break;
                case CriteriaOperator.Greater:            //>
                    criteria = _propertyName + " >= @" + _propertyName;
                    break;
                case CriteriaOperator.Like:               // %%
                    criteria = _propertyName + " like '@" + _propertyName + "'";
                    break;
                case CriteriaOperator.NotLike:               // %%
                    criteria = _propertyName + " not like '@" + _propertyName + "'";
                    break;
                default:
                    throw new ApplicationException("No operator defined.");
            }

            //debugstring += "{@" + _propertyName + "}";
            //_parameterlist.Add(_value);

            return " " + criteria + " ";
        }


        public override string GetSpecValue()
        {
            if (String.IsNullOrEmpty(_propertyName))
                return "";

           string criteria = "";

           object strCriteria = _value;


            switch (_criteriaOperator)
            {
                case CriteriaOperator.Less:
                    criteria = _propertyName + " < @";
                    break;
                case CriteriaOperator.LessThanOrEqual:    // <=
                    criteria = _propertyName + " <= @";
                    break;
                case CriteriaOperator.Equal:              // =
                    criteria = _propertyName + " = @";
                    break;
                case CriteriaOperator.NotEqual:           //!=
                    criteria = _propertyName + " != @";
                    break;
                case CriteriaOperator.GreaterThanOrEqual: // >=
                    criteria = _propertyName + " >= @";
                    break;
                case CriteriaOperator.Greater:            //>
                    criteria = _propertyName + " >= @";
                    break;
                case CriteriaOperator.Like:               // %%
                    criteria = _propertyName + " like @";
                    strCriteria = "%" + _value + "%";
                    break;

                case CriteriaOperator.NotLike:               // %%
                    criteria = _propertyName + " not like @";
                    strCriteria = "%" + _value + "%";
                    break;
                default:
                    throw new ApplicationException("No operator defined.");
            }

            if (_value.GetType() == typeof(string))
                criteria = criteria.Replace("@", "'" + strCriteria.ToString() + "'");
            else if (_value.GetType() == typeof(bool))
                criteria = criteria.Replace("@", "'" + _value.ToString() + "'");
            else if (_value.GetType() == typeof(DateTime))
                criteria = criteria.Replace("@", "cast('" + _value.ToString() + "' as DateTime)");
            else criteria = criteria.Replace("@", _value.ToString());

            return " " + criteria + " ";
        }


        public static IParametersSpecification GetTrue()
        {
            return new CriteriaSpecification("1", Models.CriteriaOperator.Equal, 1);
        }

        public static IParametersSpecification GetFalse()
        {
            return new CriteriaSpecification("1", Models.CriteriaOperator.Equal, 0);
        }

      
    }  


}
