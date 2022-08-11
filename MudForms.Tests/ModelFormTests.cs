﻿using MudBlazor.Forms;
using MudForms.Documentation.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Elemental.Tests
{
    public class ModelFormTests
    {
        [Fact]
        public void TestStarshipCategories()
        {
            var s1 = typeof(Starship).GetMudModelFormCategories();
            Assert.Single(s1);
            var (_, l1) = s1[0];
            Assert.Equal(12, l1[0].Count);

        }

        [Fact]
        public void TestConvertNullValue()
        {
            var transportStarship = new TransportStarship() { SubLightEngines = 5 };
            var prop = typeof(TransportStarship).GetProperty(nameof(TransportStarship.SubLightEngines));
            var v = MudModelFormTools.GetNonNullableValue(prop, transportStarship);
            Assert.Equal(typeof(int), v.GetType());
        }

        [Fact]
        public void TestTransportStarshipCategories()
        {
            var s1 = typeof(TransportStarship).GetMudModelFormCategories();
            Assert.Equal(3, s1.Count);
            Assert.Collection(s1, e => Assert.Null(e.category), e => Assert.Equal("Identification", e.category), e => Assert.Equal("Details", e.category));
            //var (c1, l1) = s1[0];
        }

        [Fact]
        public void TestTransportStarshipCategoriesVisibility()
        {
            var model = new ModelFormContext<TransportStarship>();
            Assert.Equal(3, model.GetCategories().Count);
            model.SetVisible<TransportStarship>(p => p.Identifier, false);

            var allCats = typeof(TransportStarship).GetMudModelFormCategories().Select(elem => (elem.category,
                    visibleProperties:elem.properties.Select(l => l.Where(l => model.IsVisible(l)).ToList()).ToList())).ToList();
            var visibleProperties = allCats.Where(p => p.visibleProperties.Any(l => l.Count > 0)).ToList();
            //        .Where(elem => elem.Item2.Count > 0).ToList();
            Assert.Equal(2, model.GetCategories().Count);
            //var (c1, l1) = s1[0];
        }

        [Fact]
        public void TestPropertyName()
        {
            Assert.Equal("Classification", MudModelFormTools.WithPropertyExpression<InteractiveStarship>(p => p.Classification).Name);
        }

    }
}
