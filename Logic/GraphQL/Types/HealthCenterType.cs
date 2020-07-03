using CoTEC_Server.DBModels;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoTEC_Server.Logic.GraphQL.Types
{
    public class HealthCenterType : ObjectType<Hospital>
    {

        protected override void Configure(IObjectTypeDescriptor<Hospital> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Name("HealthCenter");

            descriptor.BindFieldsExplicitly();

            descriptor.Field(f => f.Name).Type<NonNullType<StringType>>();

            descriptor.Field(f => f.Capacity).Type<NonNullType<IntType>>();

            descriptor.Field(f => f.IcuCapacity).Type<NonNullType<IntType>>()
                .Name("totalICUBeds");

            descriptor.Field(f => f.ManagerNavigation).Type<NonNullType<ContactType>>()
                .Name("directorContact");

            descriptor.Field(f => f.RegionNavigation).Type<NonNullType<RegionType>>()
                .Name("ubication");
        }



    }
}
