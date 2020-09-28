using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;
using CluedIn.Crawling.Factories;
using CluedIn.Crawling.Helpers;
using CluedIn.Crawling.AdventureWorksPerson.Vocabularies;
using CluedIn.Crawling.AdventureWorksPerson.Core.Models;
using CluedIn.Crawling.AdventureWorksPerson.Core;
using CluedIn.Core;
using RuleConstants = CluedIn.Core.Constants.Validation.Rules;
using System.Linq;
using System;

namespace CluedIn.Crawling.AdventureWorksPerson.ClueProducers
{
    public class PersonBusinessEntityAddressClueProducer : BaseClueProducer<PersonBusinessEntityAddress>
    {
        private readonly IClueFactory _factory;

        public PersonBusinessEntityAddressClueProducer(IClueFactory factory)
        {
            _factory = factory;
        }

        protected override Clue MakeClueImpl(PersonBusinessEntityAddress input, Guid id)
        {

            var clue = _factory.Create("/PersonBusinessEntityAddress", $"{input.Rowguid}", id);

            var data = clue.Data.EntityData;

            data.Name = $"EntityAddress {input.BusinessEntityID}";

            data.Codes.Add(new EntityCode("/PersonBusinessEntityAddress", AdventureWorksPersonConstants.CodeOrigin, $"{input.BusinessEntityID}.{input.AddressID}.{input.AddressTypeID}"));

            data.ModifiedDate = input.ModifiedDate.ParseAsDateTimeOffset(); 
            //add edges

            if (input.BusinessEntityID != null && !string.IsNullOrEmpty(input.BusinessEntityID.ToString()))
            {
                _factory.CreateOutgoingEntityReference(clue, "/PersonBusinessEntity", EntityEdgeType.For, input.BusinessEntityID, input.BusinessEntityID.ToString());
            }
            if (input.AddressID != null && !string.IsNullOrEmpty(input.AddressID.ToString()))
            {
                _factory.CreateOutgoingEntityReference(clue, "/PersonAddress", "Is", input.AddressID, input.AddressID.ToString());
            }
            if (input.AddressTypeID != null && !string.IsNullOrEmpty(input.AddressTypeID.ToString()))
            {
                _factory.CreateOutgoingEntityReference(clue, "/PersonAddressType", EntityEdgeType.IsType, input.AddressTypeID, input.AddressTypeID.ToString());
            }

            if (!data.OutgoingEdges.Any())
            {
                _factory.CreateEntityRootReference(clue, EntityEdgeType.PartOf);
            }


            var vocab = new PersonBusinessEntityAddressVocabulary();

            data.Properties[vocab.BusinessEntityID] = input.BusinessEntityID.PrintIfAvailable();
            data.Properties[vocab.AddressID] = input.AddressID.PrintIfAvailable();
            data.Properties[vocab.AddressTypeID] = input.AddressTypeID.PrintIfAvailable();
            data.Properties[vocab.Rowguid] = input.Rowguid.PrintIfAvailable();
            data.Properties[vocab.ModifiedDate] = input.ModifiedDate.PrintIfAvailable();

            clue.ValidationRuleSuppressions.AddRange(new[]
                                        {
                                RuleConstants.METADATA_001_Name_MustBeSet,
                                RuleConstants.PROPERTIES_001_MustExist,
                                RuleConstants.METADATA_002_Uri_MustBeSet,
                                RuleConstants.METADATA_003_Author_Name_MustBeSet,
                                RuleConstants.METADATA_005_PreviewImage_RawData_MustBeSet
                            });

            return clue;
        }
    }
}

