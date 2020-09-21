﻿namespace VTEX.GoodPractices
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Class UpdatePriceInfoSKUException. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="Exception" />

    [Serializable]
    public class UpdatePriceInfoSkuException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdatePriceInfoSkuException"/> class.
        /// </summary>
        /// <param name="skuId">The sku identifier.</param>
        /// <param name="innerException">The inner exception.</param>
        public UpdatePriceInfoSkuException(int skuId, Exception innerException)
            : base($"Unable to update the price of SKU {skuId} in VTEX platform", innerException)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdatePriceInfoSkuException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected UpdatePriceInfoSkuException(SerializationInfo info, StreamingContext context) : base(info, context)
        { }
    }
}
