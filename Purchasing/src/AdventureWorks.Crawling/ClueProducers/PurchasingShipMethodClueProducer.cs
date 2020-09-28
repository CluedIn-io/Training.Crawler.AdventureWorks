using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;
using CluedIn.Crawling.Factories;
using CluedIn.Crawling.Helpers;
using CluedIn.Crawling.AdventureWorksPurchasing.Vocabularies;
using CluedIn.Crawling.AdventureWorksPurchasing.Core.Models;
using CluedIn.Crawling.AdventureWorksPurchasing.Core;
using CluedIn.Core;
using RuleConstants = CluedIn.Core.Constants.Validation.Rules;
using System.Linq;
using System;

namespace CluedIn.Crawling.AdventureWorksPurchasing.ClueProducers
{
    public class PurchasingShipMethodClueProducer : BaseClueProducer<PurchasingShipMethod>
    {
        private readonly IClueFactory _factory;

        public PurchasingShipMethodClueProducer(IClueFactory factory)
        {
            _factory = factory;
        }

        protected override Clue MakeClueImpl(PurchasingShipMethod input, Guid id)
        {

            var clue = _factory.Create("/PurchasingShipMethod", $"{input.Rowguid}", id);

            var data = clue.Data.EntityData;



            data.Name = input.Name;

            data.Codes.Add(new EntityCode("/PurchasingShipMethod", AdventureWorksPurchasingConstants.CodeOrigin, $"{input.ShipMethodID}"));

            data.ModifiedDate = input.ModifiedDate.ParseAsDateTimeOffset(); 
            //add edges


            if (!data.OutgoingEdges.Any())
            {
                _factory.CreateEntityRootReference(clue, EntityEdgeType.PartOf);
            }


            var vocab = new PurchasingShipMethodVocabulary();

            data.Properties[vocab.ShipMethodID] = input.ShipMethodID.PrintIfAvailable();
            data.Properties[vocab.Name] = input.Name.PrintIfAvailable();
            data.Properties[vocab.ShipBase] = input.ShipBase.PrintIfAvailable();
            data.Properties[vocab.ShipRate] = input.ShipRate.PrintIfAvailable();
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

