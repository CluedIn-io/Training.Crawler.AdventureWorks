using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;
using CluedIn.Crawling.Factories;
using CluedIn.Crawling.Helpers;
using CluedIn.Crawling.AdventureWorks.Vocabularies;
using CluedIn.Crawling.AdventureWorks.Core.Models;
using CluedIn.Core;
using RuleConstants = CluedIn.Core.Constants.Validation.Rules;
using System.Linq;
using System;

namespace CluedIn.Crawling.AdventureWorks.ClueProducers
{
public class SalesPersonCreditCardClueProducer : BaseClueProducer<SalesPersonCreditCard>
{
private readonly IClueFactory _factory;

public SalesPersonCreditCardClueProducer(IClueFactory factory)
							{
								_factory = factory;
							}

protected override Clue MakeClueImpl(SalesPersonCreditCard input, Guid id)
{

var clue = _factory.Create("/SalesPersonCreditCard", $"{input.BusinessEntityID}.{input.CreditCardID}", id);

							var data = clue.Data.EntityData;

							

//add edges

if(input.BusinessEntityID != null && !string.IsNullOrEmpty(input.BusinessEntityID.ToString()))
{
_factory.CreateOutgoingEntityReference(clue, "/PersonBusinessEntity", EntityEdgeType.AttachedTo, input.BusinessEntityID, input.BusinessEntityID.ToString());
}
if(input.CreditCardID != null && !string.IsNullOrEmpty(input.CreditCardID.ToString()))
{
_factory.CreateOutgoingEntityReference(clue, "/SalesCreditCard", EntityEdgeType.AttachedTo, input.CreditCardID, input.CreditCardID.ToString());
}

if (!data.OutgoingEdges.Any())
			                _factory.CreateEntityRootReference(clue, EntityEdgeType.PartOf);
							

var vocab = new SalesPersonCreditCardVocabulary();

data.Properties[vocab.BusinessEntityID]          = input.BusinessEntityID.PrintIfAvailable();
data.Properties[vocab.CreditCardID]              = input.CreditCardID.PrintIfAvailable();
data.Properties[vocab.ModifiedDate]              = input.ModifiedDate.PrintIfAvailable();

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


