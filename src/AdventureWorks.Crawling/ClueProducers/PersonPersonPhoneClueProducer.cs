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
public class PersonPersonPhoneClueProducer : BaseClueProducer<PersonPersonPhone>
{
private readonly IClueFactory _factory;

public PersonPersonPhoneClueProducer(IClueFactory factory)
							{
								_factory = factory;
							}

protected override Clue MakeClueImpl(PersonPersonPhone input, Guid id)
{

var clue = _factory.Create("/PersonPersonPhone", $"{input.BusinessEntityID}.{input.PhoneNumber}.{input.PhoneNumberTypeID}", id);

							var data = clue.Data.EntityData;

							

//add edges

if(input.BusinessEntityID != null && !string.IsNullOrEmpty(input.BusinessEntityID.ToString()))
{
_factory.CreateOutgoingEntityReference(clue, "/PersonBusinessEntity", EntityEdgeType.AttachedTo, input.BusinessEntityID, input.BusinessEntityID.ToString());
}
if(input.PhoneNumberTypeID != null && !string.IsNullOrEmpty(input.PhoneNumberTypeID.ToString()))
{
_factory.CreateOutgoingEntityReference(clue, "/PersonPhoneNumberType", EntityEdgeType.AttachedTo, input.PhoneNumberTypeID, input.PhoneNumberTypeID.ToString());
}

if (!data.OutgoingEdges.Any())
			                _factory.CreateEntityRootReference(clue, EntityEdgeType.PartOf);
							

var vocab = new PersonPersonPhoneVocabulary();

data.Properties[vocab.BusinessEntityID]          = input.BusinessEntityID.PrintIfAvailable();
data.Properties[vocab.PhoneNumber]               = input.PhoneNumber.PrintIfAvailable();
data.Properties[vocab.PhoneNumberTypeID]         = input.PhoneNumberTypeID.PrintIfAvailable();
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


