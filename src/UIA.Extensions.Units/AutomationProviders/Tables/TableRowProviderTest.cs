﻿using System.Collections.Generic;
using System.Linq;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using UIA.Extensions.AutomationProviders.Interfaces.Tables;
using UIA.Extensions.AutomationProviders.Tables.Stubs;
using UIA.Extensions.InternalExtensions;

namespace UIA.Extensions.AutomationProviders.Tables
{
    [TestFixture]
    public class TableRowProviderTest
    {
        private RowInformationStub _rowInformationStub;
        private Mock<AutomationProvider> _parent;
        private readonly List<CellInformation> _values = new List<CellInformation>();

        [SetUp]
        public void SetUp()
        {
            _provider = null;
            _rowInformationStub = new RowInformationStub(_values);
            _parent = new Mock<AutomationProvider>();
        }

        [Test]
        public void ItIsOfTheDataItemIlk()
        {
            TableRowProvider.GetPropertyValue(AutomationElementIdentifiers.ControlTypeProperty.Id)
                .ShouldBeEquivalentTo(ControlType.DataItem.Id);
        }

        [Test]
        public void ItDoublesAsASelectionItem()
        {
            TableRowProvider.Should().BeAssignableTo<ISelectionItemProvider>();
            TableRowProvider.GetPatternProvider(SelectionItemPatternIdentifiers.Pattern.Id).Should()
                .BeSameAs(TableRowProvider);
        }

        [Test]
        public void ItIsSelectable()
        {
            TableRowProvider.Select();

            _rowInformationStub.ShouldHaveBeenSelected();
        }

        [Test]
        public void ItCanAddToTheSelection()
        {
            TableRowProvider.AddToSelection();
            _rowInformationStub.ShouldHaveBeenSelected();
        }

        [Test]
        public void ItCanClearTheSelection()
        {
            TableRowProvider.RemoveFromSelection();
            _rowInformationStub.ShouldHaveBeenCleared();
        }

        [Test]
        public void ItKnowsIfItIsSelected()
        {
            TableRowProvider.Select();

            TableRowProvider.IsSelected.Should().BeTrue();
        }

        [Test]
        public void TheRowValuesAreTheChildren()
        {
            AddCells("item 1", "item 2", "item 3");
            TableRowProvider.Children.Count.ShouldBeEquivalentTo(3);
            TableRowProvider.Children.Select(x => x.Name).ShouldBeEquivalentTo(new[] { "item 1", "item 2", "item 3" });
        }

        [Test]
        public void RowsWithTheSameValueAndChildrenAreEqual()
        {
            var oneRow = RowWithCells("item 1", "item 2");
            var sameRow = RowWithCells("item 1", "item 2");

            oneRow.ShouldBeEquivalentTo(sameRow);
        }

        private TableRowProvider _provider;
        private TableRowProvider TableRowProvider
        {
            get { return _provider ?? (_provider = new TableRowProvider(_parent.Object, _rowInformationStub)); }
        }

        private void AddCells(params string[] values)
        {
            values.ForEach(x => _values.Add(new CellInformationStub(x)));
        }

        private TableRowProvider RowWithCells(params string[] values)
        {
            var rowInformation = new RowInformationStub();
            values.ForEach(x => rowInformation.Cells.Add(new CellInformationStub(x)));
            return new TableRowProvider(_parent.Object, rowInformation); 
        }
    }
}
