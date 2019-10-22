﻿//------------------------------------------------------------------------------ 
// <auto-generated> 
// This code was generated by a tool. 
// 
// Changes to this file may cause incorrect behavior and will be lost if 
// the code is regenerated. 
// </auto-generated> 
//------------------------------------------------------------------------------

namespace Catel.Reflection
{
	using System;
    using Catel.Data;

	public partial class FastMemberInvoker<TEntity>
	{
        public bool SetPropertyValue<TValue>(object entity, string fieldName, TValue value)
        {
            if (typeof(TValue) == typeof(Object))
            {
                var finalValue = (object)value;

                if (SetPropertyValue((TEntity)entity, fieldName, finalValue))
                {
                    return true;
                }
            }

            if (typeof(TValue) == typeof(Boolean))
            {
                var finalValue = Convert.ToBoolean(value);

                if (SetPropertyValue((TEntity)entity, fieldName, finalValue))
                {
                    return true;
                }
            }

            if (typeof(TValue) == typeof(Char))
            {
                var finalValue = Convert.ToChar(value);

                if (SetPropertyValue((TEntity)entity, fieldName, finalValue))
                {
                    return true;
                }
            }

            if (typeof(TValue) == typeof(SByte))
            {
                var finalValue = Convert.ToSByte(value);

                if (SetPropertyValue((TEntity)entity, fieldName, finalValue))
                {
                    return true;
                }
            }

            if (typeof(TValue) == typeof(Byte))
            {
                var finalValue = Convert.ToByte(value);

                if (SetPropertyValue((TEntity)entity, fieldName, finalValue))
                {
                    return true;
                }
            }

            if (typeof(TValue) == typeof(Int16))
            {
                var finalValue = Convert.ToInt16(value);

                if (SetPropertyValue((TEntity)entity, fieldName, finalValue))
                {
                    return true;
                }
            }

            if (typeof(TValue) == typeof(UInt16))
            {
                var finalValue = Convert.ToUInt16(value);

                if (SetPropertyValue((TEntity)entity, fieldName, finalValue))
                {
                    return true;
                }
            }

            if (typeof(TValue) == typeof(Int32))
            {
                var finalValue = Convert.ToInt32(value);

                if (SetPropertyValue((TEntity)entity, fieldName, finalValue))
                {
                    return true;
                }
            }

            if (typeof(TValue) == typeof(UInt32))
            {
                var finalValue = Convert.ToUInt32(value);

                if (SetPropertyValue((TEntity)entity, fieldName, finalValue))
                {
                    return true;
                }
            }

            if (typeof(TValue) == typeof(Int64))
            {
                var finalValue = Convert.ToInt64(value);

                if (SetPropertyValue((TEntity)entity, fieldName, finalValue))
                {
                    return true;
                }
            }

            if (typeof(TValue) == typeof(UInt64))
            {
                var finalValue = Convert.ToUInt64(value);

                if (SetPropertyValue((TEntity)entity, fieldName, finalValue))
                {
                    return true;
                }
            }

            if (typeof(TValue) == typeof(Single))
            {
                var finalValue = Convert.ToSingle(value);

                if (SetPropertyValue((TEntity)entity, fieldName, finalValue))
                {
                    return true;
                }
            }

            if (typeof(TValue) == typeof(Double))
            {
                var finalValue = Convert.ToDouble(value);

                if (SetPropertyValue((TEntity)entity, fieldName, finalValue))
                {
                    return true;
                }
            }

            if (typeof(TValue) == typeof(Decimal))
            {
                var finalValue = Convert.ToDecimal(value);

                if (SetPropertyValue((TEntity)entity, fieldName, finalValue))
                {
                    return true;
                }
            }

            if (typeof(TValue) == typeof(DateTime))
            {
                var finalValue = Convert.ToDateTime(value);

                if (SetPropertyValue((TEntity)entity, fieldName, finalValue))
                {
                    return true;
                }
            }

            if (typeof(TValue) == typeof(String))
            {
                var finalValue = Convert.ToString(value);

                if (SetPropertyValue((TEntity)entity, fieldName, finalValue))
                {
                    return true;
                }
            }

            return false;
        }

        public bool SetPropertyValue(TEntity entity, string propertyName, Object value)
        {
            var setter = GetObjectPropertySetter(propertyName);
            if (setter != null)
            {
                setter(entity, value);
                return true;
            }

            return false;
        }

        public bool SetPropertyValue(TEntity entity, string propertyName, Boolean value)
        {
            var setter = GetBooleanPropertySetter(propertyName);
            if (setter != null)
            {
                setter(entity, value);
                return true;
            }

            return false;
        }

        public bool SetPropertyValue(TEntity entity, string propertyName, Char value)
        {
            var setter = GetCharPropertySetter(propertyName);
            if (setter != null)
            {
                setter(entity, value);
                return true;
            }

            return false;
        }

        public bool SetPropertyValue(TEntity entity, string propertyName, SByte value)
        {
            var setter = GetSBytePropertySetter(propertyName);
            if (setter != null)
            {
                setter(entity, value);
                return true;
            }

            return false;
        }

        public bool SetPropertyValue(TEntity entity, string propertyName, Byte value)
        {
            var setter = GetBytePropertySetter(propertyName);
            if (setter != null)
            {
                setter(entity, value);
                return true;
            }

            return false;
        }

        public bool SetPropertyValue(TEntity entity, string propertyName, Int16 value)
        {
            var setter = GetInt16PropertySetter(propertyName);
            if (setter != null)
            {
                setter(entity, value);
                return true;
            }

            return false;
        }

        public bool SetPropertyValue(TEntity entity, string propertyName, UInt16 value)
        {
            var setter = GetUInt16PropertySetter(propertyName);
            if (setter != null)
            {
                setter(entity, value);
                return true;
            }

            return false;
        }

        public bool SetPropertyValue(TEntity entity, string propertyName, Int32 value)
        {
            var setter = GetInt32PropertySetter(propertyName);
            if (setter != null)
            {
                setter(entity, value);
                return true;
            }

            return false;
        }

        public bool SetPropertyValue(TEntity entity, string propertyName, UInt32 value)
        {
            var setter = GetUInt32PropertySetter(propertyName);
            if (setter != null)
            {
                setter(entity, value);
                return true;
            }

            return false;
        }

        public bool SetPropertyValue(TEntity entity, string propertyName, Int64 value)
        {
            var setter = GetInt64PropertySetter(propertyName);
            if (setter != null)
            {
                setter(entity, value);
                return true;
            }

            return false;
        }

        public bool SetPropertyValue(TEntity entity, string propertyName, UInt64 value)
        {
            var setter = GetUInt64PropertySetter(propertyName);
            if (setter != null)
            {
                setter(entity, value);
                return true;
            }

            return false;
        }

        public bool SetPropertyValue(TEntity entity, string propertyName, Single value)
        {
            var setter = GetSinglePropertySetter(propertyName);
            if (setter != null)
            {
                setter(entity, value);
                return true;
            }

            return false;
        }

        public bool SetPropertyValue(TEntity entity, string propertyName, Double value)
        {
            var setter = GetDoublePropertySetter(propertyName);
            if (setter != null)
            {
                setter(entity, value);
                return true;
            }

            return false;
        }

        public bool SetPropertyValue(TEntity entity, string propertyName, Decimal value)
        {
            var setter = GetDecimalPropertySetter(propertyName);
            if (setter != null)
            {
                setter(entity, value);
                return true;
            }

            return false;
        }

        public bool SetPropertyValue(TEntity entity, string propertyName, DateTime value)
        {
            var setter = GetDateTimePropertySetter(propertyName);
            if (setter != null)
            {
                setter(entity, value);
                return true;
            }

            return false;
        }

        public bool SetPropertyValue(TEntity entity, string propertyName, String value)
        {
            var setter = GetStringPropertySetter(propertyName);
            if (setter != null)
            {
                setter(entity, value);
                return true;
            }

            return false;
        }

	}
}