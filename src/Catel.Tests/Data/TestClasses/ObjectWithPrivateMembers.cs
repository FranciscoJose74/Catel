﻿namespace Catel.Tests.Data
{
    using System;
    using Catel.Data;

    [Serializable]
    public class ObjectWithPrivateMembers : ComparableModelBase
    {
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public ObjectWithPrivateMembers()
        {
        }

        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public ObjectWithPrivateMembers(string privateMemberValue)
        {
            // Store values
            PrivateMember = privateMemberValue;
        }

        /// <summary>
        ///   Gets or sets the public member.
        /// </summary>
        public string PublicMember
        {
            get { return GetValue<string>(PublicMemberProperty); }
            set { SetValue(PublicMemberProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData PublicMemberProperty = RegisterProperty("PublicMember", "Public member");

        /// <summary>
        ///   Gets or sets the private member.
        /// </summary>
        private string PrivateMember
        {
            get { return GetValue<string>(PrivateMemberProperty); }
            set { SetValue(PrivateMemberProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData PrivateMemberProperty = RegisterProperty("PrivateMember", "Private member");
    }
}
