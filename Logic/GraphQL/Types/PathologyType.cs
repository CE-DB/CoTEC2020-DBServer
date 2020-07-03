using CoTEC_Server.DBModels;
using HotChocolate.Types;
using System.Collections.Generic;
using System.Linq;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class PathologyType : ObjectType<Pathology>
    {

        protected override void Configure(IObjectTypeDescriptor<Pathology> descriptor)
        {
            base.Configure(descriptor);

            descriptor.BindFieldsExplicitly();

            descriptor.Field(f => f.Name).Type<NonNullType<StringType>>();

            descriptor.Field(f => f.Description).Type<StringType>();

            descriptor.Field(f => f.Treatment).Type<StringType>();

            //descriptor.Field(f => f.PathologySymptoms).Type<NonNullType<StringType>>();

            descriptor.Field("symptoms")
                .Type<NonNullType<ListType<NonNullType<StringType>>>>()
                .Resolver(ctx => {

                    var data = ctx.Service<CoTEC_DBContext>().PathologySymptoms
                    .Where(s => s.Pathology.Equals(ctx.Parent<Pathology>().Name))
                    .Select(r => new
                    {

                       r.Symptom

                    })
                    .ToList();

                    List<string> result = new List<string>();

                    foreach(var elm in data)
                    {
                        result.Add(elm.Symptom);

                    }

                    return result;
                
                });


        }


    }
}
