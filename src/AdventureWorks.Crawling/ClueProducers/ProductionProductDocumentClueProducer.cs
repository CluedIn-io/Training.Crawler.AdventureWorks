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
public class ProductionProductDocumentClueProducer : BaseClueProducer<ProductionProductDocument>
{
private readonly IClueFactory _factory;

public ProductionProductDocumentClueProducer(IClueFactory factory)
							{
								_factory = factory;
							}

protected override Clue MakeClueImpl(ProductionProductDocument input, Guid id)
{

var clue = _factory.Create("/ProductionProductDocument", $"{input.ProductID}.{input.DocumentNode}", id);

							var data = clue.Data.EntityData;

							

//add edges

if(input.ProductID != null && !string.IsNullOrEmpty(input.ProductID.ToString()))
{
_factory.CreateOutgoingEntityReference(clue, "/ProductionProduct", EntityEdgeType.AttachedTo, input.ProductID, input.ProductID.ToString());
}
if(input.DocumentNode != null && !string.IsNullOrEmpty(input.DocumentNode.ToString()))
{
_factory.CreateOutgoingEntityReference(clue, "/ProductionDocument", EntityEdgeType.AttachedTo, input.DocumentNode, input.DocumentNode.ToString());
}

if (!data.OutgoingEdges.Any())
			                _factory.CreateEntityRootReference(clue, EntityEdgeType.PartOf);
							

var vocab = new ProductionProductDocumentVocabulary();

data.Properties[vocab.ProductID]                 = input.ProductID.PrintIfAvailable();
data.Properties[vocab.DocumentNode]              = input.DocumentNode.PrintIfAvailable();
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


