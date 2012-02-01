﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using ICSharpCode.NRefactory.TypeSystem;
using Moq;
using NUnit.Framework;
using Saltarelle.Compiler.JSModel.Expressions;

namespace Saltarelle.Compiler.Tests {
    [TestFixture]
    public class MemberConversionTests : CompilerTestBase {
        [Test]
        public void SimpleInstanceMethodCanBeConverted() {
            Compile(new[] { "class C { public void M() {} }" });
            var m = FindInstanceMethod("C.M");
            m.Definition.Should().NotBeNull();
        }

        [Test]
        public void SimpleStaticMethodCanBeConverted() {
            Compile(new[] { "class C { public static void M() {} }" });
            var m = FindStaticMethod("C.M");
            m.Definition.Should().NotBeNull();
        }

        [Test]
        public void NamingConventionIsUsedToDetermineMethodNameAndStaticity() {
            var namingConvention = new MockNamingConventionResolver { GetMethodImplementation = method => MethodImplOptions.InstanceMethod("X") };
            Compile(new[] { "class C { public static void M() {}" }, namingConvention: namingConvention);
            var m = FindInstanceMethod("C.X");
            m.Should().NotBeNull();

            namingConvention = new MockNamingConventionResolver { GetMethodImplementation = method => MethodImplOptions.StaticMethod("Y") };
            Compile(new[] { "class C { public void M() {}" }, namingConvention: namingConvention);
            m = FindStaticMethod("C.Y");
            m.Should().NotBeNull();
        }

        [Test]
        public void MethodImplementedAsInlineCodeDoesNotAppearOnTheType() {
            var namingConvention = new MockNamingConventionResolver { GetMethodImplementation = method => MethodImplOptions.InlineCode("X") };
            Compile(new[] { "class C { public static void M() {}" }, namingConvention: namingConvention);
            FindClass("C").InstanceMethods.Should().BeEmpty();
        }

        [Test]
        public void MethodImplementedAsInstanceMethodOnFirstArgumentDoesNotAppearOnTheType() {
            var namingConvention = new MockNamingConventionResolver { GetMethodImplementation = method => MethodImplOptions.InstanceMethodOnFirstArgument("X") };
            Compile(new[] { "class C { public static void M() {}" }, namingConvention: namingConvention);
            FindClass("C").InstanceMethods.Should().BeEmpty();
        }

        [Test]
        public void MethodImplementedAsNotUsableFromScriptDoesNotAppearOnTheType() {
            var namingConvention = new MockNamingConventionResolver { GetMethodImplementation = method => MethodImplOptions.NotUsableFromScript() };
            Compile(new[] { "class C { public static void M() {}" }, namingConvention: namingConvention);
            FindClass("C").InstanceMethods.Should().BeEmpty();
        }

        [Test]
        public void InstanceMethodWithGenerateCodeSetToFalseDoesNotAppearOnTheType() {
            var namingConvention = new MockNamingConventionResolver { GetMethodImplementation = method => MethodImplOptions.InstanceMethod("X", generateCode: false) };
            Compile(new[] { "class C { public static void M() {}" }, namingConvention: namingConvention);
            FindClass("C").InstanceMethods.Should().BeEmpty();
        }

        [Test]
        public void StaticMethodWithGenerateCodeSetToFalseDoesNotAppearOnTheType() {
            var namingConvention = new MockNamingConventionResolver { GetMethodImplementation = method => MethodImplOptions.StaticMethod("X", generateCode: false) };
            Compile(new[] { "class C { public static void M() {}" }, namingConvention: namingConvention);
            FindClass("C").InstanceMethods.Should().BeEmpty();
        }

        [Test]
        public void DefaultConstructorIsInsertedIfNoConstructorIsDefined() {
            Compile(new[] { "class C {}" });
            var cls = FindClass("C");
            cls.Constructors.Should().HaveCount(1);
            cls.Constructors[0].Name.Should().BeNull();
            cls.Constructors[0].Definition.Should().NotBeNull();
            cls.Constructors[0].Definition.ParameterNames.Should().HaveCount(0);
        }

        [Test]
        public void DefaultConstructorImplementedAsStaticMethodWorks() {
            var namingConvention = new MockNamingConventionResolver { GetConstructorImplementation = ctor => ConstructorImplOptions.StaticMethod("X") };
            Compile(new[] { "class C { }" }, namingConvention: namingConvention);
            FindStaticMethod("C.X").Should().NotBeNull();
            FindConstructor("C.X").Should().BeNull();
        }

        [Test]
        public void DefaultConstructorIsNotInsertedIfOtherConstructorIsDefined() {
            Compile(new[] { "class C { C(int i) {} }" });
            var cls = FindClass("C");
            cls.Constructors.Should().HaveCount(1);
            cls.Constructors[0].Name.Should().Be("ctor$Int32");
            cls.Constructors[0].Definition.Should().NotBeNull();
        }

        [Test]
        public void ConstructorsCanBeOverloadedWithDifferentImplementations() {
            var namingConvention = new MockNamingConventionResolver { GetConstructorImplementation = ctor => ctor.Parameters[0].Type.Name == "String" ? ConstructorImplOptions.Named("StringCtor") : ConstructorImplOptions.StaticMethod("IntCtor") };
            Compile(new[] { "class C { C(int i) {} C(string s) {} }" }, namingConvention: namingConvention);
            FindClass("C").Constructors.Should().HaveCount(1);
            FindClass("C").StaticMethods.Should().HaveCount(1);
            FindConstructor("C.StringCtor").Should().NotBeNull();
            FindStaticMethod("C.IntCtor").Should().NotBeNull();
        }

        [Test]
        public void ConstructorImplementedAsStaticMethodGetsAddedToTheStaticMethodsCollectionAndNotTheConstructors() {
            var namingConvention = new MockNamingConventionResolver { GetConstructorImplementation = ctor => ConstructorImplOptions.StaticMethod("X") };
            Compile(new[] { "class C { public C() {}" }, namingConvention: namingConvention);
            FindStaticMethod("C.X").Should().NotBeNull();
            FindConstructor("C.X").Should().BeNull();
        }

        [Test]
        public void ConstructorImplementedAsNotUsableFromScriptDoesNotAppearOnTheType() {
            var namingConvention = new MockNamingConventionResolver { GetConstructorImplementation = ctor => ConstructorImplOptions.NotUsableFromScript() };
            Compile(new[] { "class C { public static void M() {}" }, namingConvention: namingConvention);
            var m = FindInstanceMethod("C.X");
            m.Should().BeNull();
        }

        [Test]
        public void BaseMethodsAreNotIncludedInDerivedType() {
            Compile(new[] { "class B { public void X(); } class C : B { public void Y() {} }" });
            var cls = FindClass("C");
            cls.InstanceMethods.Should().HaveCount(1);
            cls.InstanceMethods[0].Name.Should().Be("Y");
        }

        [Test]
        public void ShadowingMethodsAreIncluded() {
            var namingConvention = new MockNamingConventionResolver { GetMethodImplementation = m => MethodImplOptions.InstanceMethod(m.DeclaringType.Name == "C" ? "XDerived" : m.Name) };
            Compile(new[] { "class B { public void X(); } class C : B { public new void X() {} }" }, namingConvention: namingConvention);
            var cls = FindClass("C");
            cls.InstanceMethods.Should().HaveCount(1);
            cls.InstanceMethods[0].Name.Should().Be("XDerived");
        }

        [Test]
        public void OverridingMethodsAreIncluded() {
            Compile(new[] { "class B { public virtual void X(); } class C : B { public override void X() {} }" });
            var cls = FindClass("C");
            cls.InstanceMethods.Should().HaveCount(1);
            cls.InstanceMethods[0].Name.Should().Be("X");
        }

        [Test]
        public void AdditionalNamesWorkForInstanceMethods() {
            var namingConvention = new MockNamingConventionResolver { GetMethodImplementation = m => MethodImplOptions.InstanceMethod("X", additionalNames: new[] { "X1", "X2" } ) };
            Compile(new[] { "class C { public void X() {} }" }, namingConvention: namingConvention);
            var cls = FindClass("C");
            cls.InstanceMethods.Should().HaveCount(3);
            cls.InstanceMethods.Should().Contain(m => m.Name == "X");
            cls.InstanceMethods.Should().Contain(m => m.Name == "X1");
            cls.InstanceMethods.Should().Contain(m => m.Name == "X2");
        }

        [Test]
        public void AdditionalNamesWorkForStaticMethods() {
            var namingConvention = new MockNamingConventionResolver { GetMethodImplementation = m => MethodImplOptions.StaticMethod("X", additionalNames: new[] { "X1", "X2" } ) };
            Compile(new[] { "class C { public static void X() {} }" }, namingConvention: namingConvention);
            var cls = FindClass("C");
            cls.StaticMethods.Should().HaveCount(3);
            cls.StaticMethods.Should().Contain(m => m.Name == "X");
            cls.StaticMethods.Should().Contain(m => m.Name == "X1");
            cls.StaticMethods.Should().Contain(m => m.Name == "X2");
        }

        [Test]
        public void OperatorsWork() {
            Compile(new[] { "class C { public static bool operator==(C a, C b) {} }" });
            FindStaticMethod("C.op_Equality").Should().NotBeNull();
        }

        [Test, Ignore("Partial methods lack support in NRefactory")]
        public void PartialMethodWithoutDefinitionIsNotImported() {
            var namingConvention = new MockNamingConventionResolver { GetMethodImplementation = m => { throw new InvalidOperationException(); } };
            Compile(new[] { "partial class C { private partial void M(); }" }, namingConvention: namingConvention);
            FindClass("C").InstanceMethods.Should().BeEmpty();
            FindClass("C").StaticMethods.Should().BeEmpty();
        }

        [Test, Ignore("Partial methods lack support in NRefactory")]
        public void OverloadedPartialMethodsWork() {
            var namingConvention = new MockNamingConventionResolver { GetMethodImplementation = m => MethodImplOptions.InstanceMethod("$M_" + m.Parameters.Count) };
            Compile(new[] { "partial class C { partial void M(); partial void M(int i); }", "partial class C { partial void M(int i) {} }" }, namingConvention: namingConvention);
            #warning TODO: Find out why code below fails
            Assert.That(FindInstanceMethod("C.$M_0"), Is.Null);
            Assert.That(FindInstanceMethod("C.$M_1"), Is.Null);
            #if FALSE
            FindInstanceMethod("C.$M_0").Should().BeNull();
            FindInstanceMethod("C.$M_1").Should().NotBeNull();
            #endif
        }

        [Test, Ignore("Partial methods lack support in NRefactory")]
        public void PartialMethodWithDeclarationAndDefinitionIsImported() {
            var namingConvention = new MockNamingConventionResolver { GetMethodImplementation = m => MethodImplOptions.InstanceMethod("$M") };
            Compile(new[] { "partial class C { partial void M(); }", "partial class C { partial void M() {} }" }, namingConvention: namingConvention);
            FindClass("C").InstanceMethods.Should().BeEmpty();
            FindClass("C").StaticMethods.Should().BeEmpty();
        }

        [Test]
        public void GenericMethodTypeArgumentsAreIncludedForInstanceMethods() {
            var namingConvention = new MockNamingConventionResolver { GetMethodImplementation = m => MethodImplOptions.InstanceMethod("X", additionalNames: new[] { "X1", "X2" }), GetTypeParameterName = tp => "$$" + tp.Name };
            Compile(new[] { "class C { public void X<U, V>() {} }" }, namingConvention: namingConvention);
            FindInstanceMethod("C.X").TypeParameterNames.Should().Equal(new[] { "$$U", "$$V" });
            FindInstanceMethod("C.X1").TypeParameterNames.Should().Equal(new[] { "$$U", "$$V" });
            FindInstanceMethod("C.X2").TypeParameterNames.Should().Equal(new[] { "$$U", "$$V" });
        }

        [Test]
        public void GenericMethodTypeArgumentsAreIgnoredForInstanceMethodsIfTheMethodImplOptionsSaySo() {
            var namingConvention = new MockNamingConventionResolver { GetMethodImplementation = m => MethodImplOptions.InstanceMethod("X", additionalNames: new[] { "X1", "X2" }, ignoreGenericArguments: true), GetTypeParameterName = tp => "$$" + tp.Name };
            Compile(new[] { "class C { public void X<U, V>() {} }" }, namingConvention: namingConvention);
            FindInstanceMethod("C.X").TypeParameterNames.Should().BeEmpty();
            FindInstanceMethod("C.X1").TypeParameterNames.Should().BeEmpty();
            FindInstanceMethod("C.X2").TypeParameterNames.Should().BeEmpty();
        }

        [Test]
        public void GenericMethodTypeArgumentsAreIncludedForStaticMethods() {
            var namingConvention = new MockNamingConventionResolver { GetMethodImplementation = m => MethodImplOptions.StaticMethod("X", additionalNames: new[] { "X1", "X2" }), GetTypeParameterName = tp => "$$" + tp.Name };
            Compile(new[] { "class C { public void X<U, V>() {} }" }, namingConvention: namingConvention);
            FindStaticMethod("C.X").TypeParameterNames.Should().Equal(new[] { "$$U", "$$V" });
            FindStaticMethod("C.X1").TypeParameterNames.Should().Equal(new[] { "$$U", "$$V" });
            FindStaticMethod("C.X2").TypeParameterNames.Should().Equal(new[] { "$$U", "$$V" });
        }

        [Test]
        public void GenericMethodTypeArgumentsAreIgnoredForStaticMethodsIfTheMethodImplOptionsSaySo() {
            var namingConvention = new MockNamingConventionResolver { GetMethodImplementation = m => MethodImplOptions.StaticMethod("X", additionalNames: new[] { "X1", "X2" }, ignoreGenericArguments: true), GetTypeParameterName = tp => "$$" + tp.Name };
            Compile(new[] { "class C { public void X<U, V>() {} }" }, namingConvention: namingConvention);
            FindStaticMethod("C.X").TypeParameterNames.Should().BeEmpty();
            FindStaticMethod("C.X1").TypeParameterNames.Should().BeEmpty();
            FindStaticMethod("C.X2").TypeParameterNames.Should().BeEmpty();
        }

        [Test]
        public void InstanceAutoPropertiesWithGetSetMethodsAndFieldAreCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.GetAndSetMethods(MethodImplOptions.InstanceMethod("get_" + p.Name), MethodImplOptions.InstanceMethod("set_" + p.Name)),
                                                                      GetAutoPropertyBackingFieldImplementation = p => FieldImplOptions.Instance("$" + p.Name)
                                                                    };

            Compile(new[] { "class C { public string SomeProp { get; set; } }" }, namingConvention: namingConvention);
            FindInstanceMethod("C.get_SomeProp").Should().NotBeNull();
            FindInstanceMethod("C.set_SomeProp").Should().NotBeNull();
            FindInstanceField("C.$SomeProp").Should().NotBeNull();
        }

        [Test]
        public void InstanceAutoPropertiesWithGetSetMethodsWithNoCodeAreCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.GetAndSetMethods(MethodImplOptions.InstanceMethod("get_" + p.Name, generateCode: false), MethodImplOptions.InstanceMethod("set_" + p.Name, generateCode: false)),
                                                                      GetAutoPropertyBackingFieldImplementation = p => FieldImplOptions.NotUsableFromScript()
            };
            Compile(new[] { "class C { public string SomeProp { get; set; } }" }, namingConvention: namingConvention);
            FindClass("C").InstanceMethods.Should().BeEmpty();
            FindClass("C").InstanceFields.Should().BeEmpty();
        }

        [Test]
        public void InstanceAutoPropertiesWithGetSetMethodsStaticAreCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.GetAndSetMethods(MethodImplOptions.StaticMethod("get_" + p.Name), MethodImplOptions.StaticMethod("set_" + p.Name)),
                                                                      GetAutoPropertyBackingFieldImplementation = p => FieldImplOptions.Static("$SomeProp")
                                                                    };
            Compile(new[] { "class C { public string SomeProp { get; set; } }" }, namingConvention: namingConvention);

            Assert.That(FindInstanceMethod("C.get_SomeProp"), Is.Null);
            Assert.That(FindInstanceMethod("C.set_SomeProp"), Is.Null);
            Assert.That(FindStaticMethod("C.get_SomeProp"), Is.Not.Null);
            Assert.That(FindStaticMethod("C.set_SomeProp"), Is.Not.Null);
            Assert.That(FindStaticField("C.$SomeProp"), Is.Not.Null);
#warning Determine why code below does not work.
#if FALSE
            FindInstanceMethod("C.get_SomeProp").Should().BeNull();
            FindInstanceMethod("C.set_SomeProp").Should().BeNull();
            FindStaticMethod("C.get_SomeProp").Should().NotBeNull();
            FindStaticMethod("C.set_SomeProp").Should().NotBeNull();
            FindClass("C").InstanceFields.Should().BeEmpty();
            FindClass("C").StaticFields.Should().BeEmpty();
#endif
        }

        [Test]
        public void InstanceAutoPropertiesWithGetSetMethodsStaticWithNoCodeAreCorrectlyImported()
        {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.GetAndSetMethods(MethodImplOptions.StaticMethod("get_" + p.Name, generateCode: false), MethodImplOptions.StaticMethod("set_" + p.Name, generateCode: false)),
                                                                      GetAutoPropertyBackingFieldImplementation = p => FieldImplOptions.NotUsableFromScript()
            };
            Compile(new[] { "class C { public string SomeProp { get; set; } }" }, namingConvention: namingConvention);

            Assert.That(FindInstanceMethod("C.get_SomeProp"), Is.Null);
            Assert.That(FindInstanceMethod("C.set_SomeProp"), Is.Null);
            Assert.That(FindStaticMethod("C.get_SomeProp"), Is.Null);
            Assert.That(FindStaticMethod("C.set_SomeProp"), Is.Null);
            Assert.That(FindClass("C").InstanceFields, Is.Empty);
            Assert.That(FindClass("C").StaticFields, Is.Empty);
#warning Determine why code below does not work.
#if FALSE
            FindInstanceMethod("C.get_SomeProp").Should().BeNull();
            FindInstanceMethod("C.set_SomeProp").Should().BeNull();
            FindStaticMethod("C.get_SomeProp").Should().NotBeNull();
            FindStaticMethod("C.set_SomeProp").Should().NotBeNull();
            FindClass("C").InstanceFields.Should().BeEmpty();
            FindClass("C").StaticFields.Should().BeEmpty();
#endif
        }

        [Test]
        public void InstanceAutoPropertiesWithInstanceFieldAreCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.GetAndSetMethods(MethodImplOptions.InstanceMethod("get_" + p.Name), MethodImplOptions.InstanceMethod("set_" + p.Name)),
                                                                      GetAutoPropertyBackingFieldImplementation = p => FieldImplOptions.Instance("$" + p.Name)
                                                                    };
            Compile(new[] { "class C { public string SomeProp { get; set; } }" }, namingConvention: namingConvention);
            FindInstanceMethod("C.get_SomeProp").Should().NotBeNull();
            FindInstanceMethod("C.set_SomeProp").Should().NotBeNull();
            FindInstanceField("C.$SomeProp").Should().NotBeNull();
            FindClass("C").StaticFields.Should().BeEmpty();
        }


        [Test]
        public void InstanceAutoPropertiesWithStaticFieldAreCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.GetAndSetMethods(MethodImplOptions.InstanceMethod("get_" + p.Name), MethodImplOptions.InstanceMethod("set_" + p.Name)),
                                                                      GetAutoPropertyBackingFieldImplementation = p => FieldImplOptions.Static("$" + p.Name)
                                                                    };
            Compile(new[] { "class C { public string SomeProp { get; set; } }" }, namingConvention: namingConvention);
            FindInstanceMethod("C.get_SomeProp").Should().NotBeNull();
            FindInstanceMethod("C.set_SomeProp").Should().NotBeNull();
            FindClass("C").InstanceFields.Should().BeEmpty();
            FindStaticField("C.$SomeProp").Should().NotBeNull();
        }

        [Test]
        public void InstanceAutoPropertiesThatShouldBeInstanceFieldsAreCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.InstanceField("$" + p.Name) };
            Compile(new[] { "class C { public string SomeProp { get; set; } }" }, namingConvention: namingConvention);
            FindClass("C").InstanceMethods.Should().BeEmpty();
            FindClass("C").StaticMethods.Should().BeEmpty();
            FindInstanceField("C.$SomeProp").Should().NotBeNull();
        }

        [Test]
        public void InstanceAutoPropertiesThatShouldBeStaticFieldsAreCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.StaticField("$" + p.Name) };
            Compile(new[] { "class C { public string SomeProp { get; set; } }" }, namingConvention: namingConvention);
            FindClass("C").InstanceMethods.Should().BeEmpty();
            FindClass("C").StaticMethods.Should().BeEmpty();
            FindStaticField("C.$SomeProp").Should().NotBeNull();
        }

        [Test]
        public void StaticAutoPropertiesWithGetSetMethodsAndFieldAreCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.GetAndSetMethods(MethodImplOptions.StaticMethod("get_" + p.Name), MethodImplOptions.StaticMethod("set_" + p.Name)),
                                                                      GetAutoPropertyBackingFieldImplementation = p => FieldImplOptions.Instance("$" + p.Name)
                                                                    };

            Compile(new[] { "class C { public string SomeProp { get; set; } }" }, namingConvention: namingConvention);
            FindStaticMethod("C.get_SomeProp").Should().NotBeNull();
            FindStaticMethod("C.set_SomeProp").Should().NotBeNull();
            FindInstanceField("C.$SomeProp").Should().NotBeNull();
        }

        [Test]
        public void StaticAutoPropertiesWithGetSetMethodsButNoFieldAreCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.GetAndSetMethods(MethodImplOptions.StaticMethod("get_" + p.Name), MethodImplOptions.StaticMethod("set_" + p.Name)),
                                                                      GetAutoPropertyBackingFieldImplementation = p => FieldImplOptions.NotUsableFromScript()
                                                                    };
            Compile(new[] { "class C { public string SomeProp { get; set; } }" }, namingConvention: namingConvention);
            FindStaticMethod("C.get_SomeProp").Should().NotBeNull();
            FindStaticMethod("C.set_SomeProp").Should().NotBeNull();
            FindClass("C").InstanceFields.Should().BeEmpty();
        }

        [Test]
        public void StaticAutoPropertiesThatShouldBeFieldsAreCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.StaticField("$" + p.Name) };
            Compile(new[] { "class C { public string SomeProp { get; set; } }" }, namingConvention: namingConvention);
            FindClass("C").InstanceMethods.Should().BeEmpty();
            FindClass("C").StaticMethods.Should().BeEmpty();
            FindStaticField("C.$SomeProp").Should().NotBeNull();
        }

        [Test]
        public void AutoPropertyBackingFieldIsCorrectlyInitialized() {
            Assert.Inconclusive("TODO");
        }

        [Test]
        public void ManuallyImplementedInstancePropertyWithGetAndSetMethodsIsCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.GetAndSetMethods(MethodImplOptions.InstanceMethod("get_SomeProp"), MethodImplOptions.InstanceMethod("set_SomeProp")) };
            Compile(new[] { "class C { public int SomeProp { get { return 0; } set {} } }" }, namingConvention: namingConvention);
            FindInstanceMethod("C.get_SomeProp").Should().NotBeNull();
            FindInstanceMethod("C.set_SomeProp").Should().NotBeNull();
            FindClass("C").StaticMethods.Should().BeEmpty();
        }

        [Test]
        public void ManuallyImplementedInstancePropertyThatShouldBeAFieldIsCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.InstanceField("$SomeProp") };
            Compile(new[] { "class C { public int SomeProp { get { return 0; } set {} } }" }, namingConvention: namingConvention);
            FindInstanceField("C.$SomeProp").Should().NotBeNull();
            FindClass("C").InstanceMethods.Should().BeEmpty();
            FindClass("C").StaticMethods.Should().BeEmpty();
        }

        [Test]
        public void ManuallyImplementedReadOnlyInstancePropertyWithGetAndSetMethodsIsCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.GetAndSetMethods(MethodImplOptions.InstanceMethod("get_SomeProp"), null) };
            Compile(new[] { "class C { public int SomeProp { get { return 0; } } }" }, namingConvention: namingConvention);
            FindInstanceMethod("C.get_SomeProp").Should().NotBeNull();
            FindInstanceMethod("C.set_SomeProp").Should().BeNull();
            FindClass("C").StaticMethods.Should().BeEmpty();
        }

        [Test]
        public void ManuallyImplementedReadOnlyInstancePropertyThatShouldBeAFieldIsCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.InstanceField("$SomeProp") };
            Compile(new[] { "class C { public int SomeProp { get { return 0; } }" }, namingConvention: namingConvention);
            FindInstanceField("C.$SomeProp").Should().NotBeNull();
            FindClass("C").InstanceMethods.Should().BeEmpty();
            FindClass("C").StaticMethods.Should().BeEmpty();
        }

        [Test]
        public void ManuallyImplementedWriteOnlyInstancePropertyWithGetAndSetMethodsIsCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.GetAndSetMethods(null, MethodImplOptions.InstanceMethod("set_SomeProp")) };
            Compile(new[] { "class C { public int SomeProp { set {} } }" }, namingConvention: namingConvention);
            FindInstanceMethod("C.get_SomeProp").Should().BeNull();
            FindInstanceMethod("C.set_SomeProp").Should().NotBeNull();
            FindClass("C").StaticMethods.Should().BeEmpty();
        }

        [Test]
        public void ManuallyImplementedWriteOnlyInstancePropertyThatShouldBeAFieldIsCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.InstanceField("$SomeProp") };
            Compile(new[] { "class C { public int SomeProp { set {} }" }, namingConvention: namingConvention);
            FindInstanceField("C.$SomeProp").Should().NotBeNull();
            FindClass("C").InstanceMethods.Should().BeEmpty();
            FindClass("C").StaticMethods.Should().BeEmpty();
        }

        [Test]
        public void ManuallyImplementedStaticPropertyWithGetAndSetMethodsIsCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.GetAndSetMethods(MethodImplOptions.StaticMethod("get_SomeProp"), MethodImplOptions.StaticMethod("set_SomeProp")) };
            Compile(new[] { "class C { public int SomeProp { get { return 0; } set {} } }" }, namingConvention: namingConvention);
            FindStaticMethod("C.get_SomeProp").Should().NotBeNull();
            FindStaticMethod("C.set_SomeProp").Should().NotBeNull();
            FindClass("C").InstanceMethods.Should().BeEmpty();
        }

        [Test]
        public void ManuallyImplementedStaticPropertyThatShouldBeAFieldIsCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.StaticField("$SomeProp") };
            Compile(new[] { "class C { public int SomeProp { get { return 0; } set {} } }" }, namingConvention: namingConvention);
            FindStaticField("C.$SomeProp").Should().NotBeNull();
            FindClass("C").InstanceMethods.Should().BeEmpty();
            FindClass("C").StaticMethods.Should().BeEmpty();
        }

        [Test]
        public void ManuallyImplementedReadOnlyStaticPropertyWithGetAndSetMethodsIsCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.GetAndSetMethods(MethodImplOptions.StaticMethod("get_SomeProp"), null) };
            Compile(new[] { "class C { public int SomeProp { get { return 0; } } }" }, namingConvention: namingConvention);
            FindStaticMethod("C.get_SomeProp").Should().NotBeNull();
            FindStaticMethod("C.set_SomeProp").Should().BeNull();
            FindClass("C").InstanceMethods.Should().BeEmpty();
        }

        [Test]
        public void ManuallyImplementedReadOnlyStaticPropertyThatShouldBeAFieldIsCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.StaticField("$SomeProp") };
            Compile(new[] { "class C { public int SomeProp { get { return 0; } }" }, namingConvention: namingConvention);
            FindStaticField("C.$SomeProp").Should().NotBeNull();
            FindClass("C").InstanceMethods.Should().BeEmpty();
            FindClass("C").StaticMethods.Should().BeEmpty();
        }

        [Test]
        public void ManuallyImplementedWriteOnlyStaticPropertyWithGetAndSetMethodsIsCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.GetAndSetMethods(null, MethodImplOptions.StaticMethod("set_SomeProp")) };
            Compile(new[] { "class C { public int SomeProp { set {} } }" }, namingConvention: namingConvention);
            FindStaticMethod("C.get_SomeProp").Should().BeNull();
            FindStaticMethod("C.set_SomeProp").Should().NotBeNull();
            FindClass("C").InstanceMethods.Should().BeEmpty();
        }

        [Test]
        public void ManuallyImplementedWriteOnlyStaticPropertyThatShouldBeAFieldIsCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.StaticField("$SomeProp") };
            Compile(new[] { "class C { public int SomeProp { set {} }" }, namingConvention: namingConvention);
            FindStaticField("C.$SomeProp").Should().NotBeNull();
            FindClass("C").InstanceMethods.Should().BeEmpty();
            FindClass("C").StaticMethods.Should().BeEmpty();
        }

        [Test]
        public void InstanceFieldsAreCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetFieldImplementation = f => FieldImplOptions.Instance("$SomeProp") };
            Compile(new[] { "class C { public int SomeField; }" }, namingConvention: namingConvention);
            FindInstanceField("C.$SomeProp").Should().NotBeNull();
            FindClass("C").StaticFields.Should().BeEmpty();
            FindClass("C").InstanceMethods.Should().BeEmpty();
            FindClass("C").StaticMethods.Should().BeEmpty();
        }

        [Test]
        public void StaticFieldsAreCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetFieldImplementation = f => FieldImplOptions.Static("$SomeProp") };
            Compile(new[] { "class C { public int SomeField; }" }, namingConvention: namingConvention);
            FindStaticField("C.$SomeProp").Should().NotBeNull();
            FindClass("C").InstanceFields.Should().BeEmpty();
            FindClass("C").InstanceMethods.Should().BeEmpty();
            FindClass("C").StaticMethods.Should().BeEmpty();
        }

        [Test]
        public void FieldsThatAreNotUsableFromScriptAreNotImported() {
            var namingConvention = new MockNamingConventionResolver { GetFieldImplementation = f => FieldImplOptions.NotUsableFromScript() };
            Compile(new[] { "class C { public int SomeField; }" }, namingConvention: namingConvention);
            FindClass("C").InstanceFields.Should().BeEmpty();
            FindClass("C").StaticFields.Should().BeEmpty();
            FindClass("C").InstanceMethods.Should().BeEmpty();
            FindClass("C").StaticMethods.Should().BeEmpty();
        }

        [Test]
        public void ImportingMultipleFieldsInTheSameDeclarationWorks() {
            var namingConvention = new MockNamingConventionResolver { GetFieldImplementation = f => FieldImplOptions.Instance("$" + f.Name) };
            Compile(new[] { "class C { public int Field1, Field2; }" }, namingConvention: namingConvention);
            FindInstanceField("C.$Field1").Should().NotBeNull();
            FindInstanceField("C.$Field2").Should().NotBeNull();
            FindClass("C").StaticFields.Should().BeEmpty();
            FindClass("C").InstanceMethods.Should().BeEmpty();
            FindClass("C").StaticMethods.Should().BeEmpty();
        }


        [Test]
        public void FieldWithoutInitializerIsInitializedToDefault() {
            Assert.Inconclusive("TODO");
        }

        [Test]
        public void FieldInitializersAreCorrectlyImported() {
            Assert.Inconclusive("TODO");
        }

        [Test]
        public void InstanceAutoEventsWithAddRemoveMethodsAreCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetEventImplementation = f => EventImplOptions.AddAndRemoveMethods(MethodImplOptions.InstanceMethod("add_" + f.Name), MethodImplOptions.InstanceMethod("remove_" + f.Name)),
                                                                      GetAutoEventBackingFieldImplementation = f => FieldImplOptions.Instance("$" + f.Name)
                                                                    };

            Compile(new[] { "class C { public event System.EventHandler SomeProp; }" }, namingConvention: namingConvention);
            FindInstanceMethod("C.add_SomeProp").Should().NotBeNull();
            FindInstanceMethod("C.remove_SomeProp").Should().NotBeNull();
            FindInstanceField("C.$SomeProp").Should().NotBeNull();
        }

        [Test]
        public void InstanceAutoEventsWithAddRemoveMethodsWithNoCodeAreCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetEventImplementation = f => EventImplOptions.AddAndRemoveMethods(MethodImplOptions.InstanceMethod("add_" + f.Name, generateCode: false), MethodImplOptions.InstanceMethod("remove_" + f.Name, generateCode: false)),
                                                                      GetAutoEventBackingFieldImplementation = f => FieldImplOptions.NotUsableFromScript()
                                                                    };
            Compile(new[] { "class C { public event System.EventHandler SomeProp; }" }, namingConvention: namingConvention);
            FindClass("C").InstanceMethods.Should().BeEmpty();
            FindClass("C").InstanceFields.Should().BeEmpty();
        }

        [Test]
        public void StaticAutoEventsWithAddRemoveMethodsAreCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetEventImplementation = f => EventImplOptions.AddAndRemoveMethods(MethodImplOptions.StaticMethod("add_" + f.Name), MethodImplOptions.StaticMethod("remove_" + f.Name)),
                                                                      GetAutoEventBackingFieldImplementation = f => FieldImplOptions.Static("$" + f.Name)
                                                                    };

            Compile(new[] { "class C { public event System.EventHandler SomeProp; }" }, namingConvention: namingConvention);
            FindStaticMethod("C.add_SomeProp").Should().NotBeNull();
            FindStaticMethod("C.remove_SomeProp").Should().NotBeNull();
            FindStaticField("C.$SomeProp").Should().NotBeNull();
        }


        [Test]
        public void InstanceManualEventsWithAddRemoveMethodsAreCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetEventImplementation = f => EventImplOptions.AddAndRemoveMethods(MethodImplOptions.InstanceMethod("add_" + f.Name), MethodImplOptions.InstanceMethod("remove_" + f.Name)) };

            Compile(new[] { "class C { public event System.EventHandler SomeProp { add {} remove{} } }" }, namingConvention: namingConvention);
            FindInstanceMethod("C.add_SomeProp").Should().NotBeNull();
            FindInstanceMethod("C.remove_SomeProp").Should().NotBeNull();
        }

        [Test]
        public void InstanceManualEventsWithAddRemoveMethodsWithNoCodeAreCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetEventImplementation = f => EventImplOptions.AddAndRemoveMethods(MethodImplOptions.InstanceMethod("add_" + f.Name, generateCode: false), MethodImplOptions.InstanceMethod("remove_" + f.Name, generateCode: false)) };
            Compile(new[] { "class C { public event System.EventHandler SomeProp { add {} remove{} } }" }, namingConvention: namingConvention);
            FindClass("C").InstanceMethods.Should().BeEmpty();
            FindClass("C").InstanceFields.Should().BeEmpty();
        }

        [Test]
        public void StaticManualEventsWithAddRemoveMethodsAreCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetEventImplementation = f => EventImplOptions.AddAndRemoveMethods(MethodImplOptions.StaticMethod("add_" + f.Name), MethodImplOptions.StaticMethod("remove_" + f.Name)) };

            Compile(new[] { "class C { public event System.EventHandler SomeProp { add {} remove{} } }" }, namingConvention: namingConvention);
            FindStaticMethod("C.add_SomeProp").Should().NotBeNull();
            FindStaticMethod("C.remove_SomeProp").Should().NotBeNull();
        }

        [Test]
        public void ImportingMultipleEventsInTheSameDeclarationWorks() {
            var namingConvention = new MockNamingConventionResolver { GetEventImplementation = f => EventImplOptions.AddAndRemoveMethods(MethodImplOptions.InstanceMethod("add_" + f.Name), MethodImplOptions.InstanceMethod("remove_" + f.Name)),
                                                                      GetAutoEventBackingFieldImplementation = f => FieldImplOptions.Instance("$" + f.Name)
                                                                    };
            Compile(new[] { "class C { public event System.EventHandler Event1, Event2; }" }, namingConvention: namingConvention);
            FindInstanceField("C.$Event1").Should().NotBeNull();
            FindInstanceField("C.$Event2").Should().NotBeNull();
            FindInstanceMethod("C.add_Event1").Should().NotBeNull();
            FindInstanceMethod("C.remove_Event1").Should().NotBeNull();
            FindInstanceMethod("C.add_Event2").Should().NotBeNull();
            FindInstanceMethod("C.remove_Event2").Should().NotBeNull();
            FindClass("C").StaticFields.Should().BeEmpty();
            FindClass("C").StaticMethods.Should().BeEmpty();
        }

        [Test]
        public void BackingFieldsForAutoEventsWithInitializerUseThatInitializer() {
            Assert.Inconclusive("TODO");
        }

        [Test]
        public void BackingFieldsForAutoEventsWithNoInitializerGetInitializedToDefault() {
            Assert.Inconclusive("TODO");
        }

        [Test]
        public void EmptyEnumIsCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetEnumValueName = v => "$" + v.Name };
            Compile(new[] { "enum MyEnum {}" }, namingConvention: namingConvention);
            FindEnum("MyEnum").Values.Should().BeEmpty();
        }

        [Test]
        public void EnumWithMembersIsCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetEnumValueName = v => "$" + v.Name };
            Compile(new[] { "enum MyEnum { Member1, Member2 }" }, namingConvention: namingConvention);
            FindEnum("MyEnum").Values.Should().HaveCount(2);
            FindEnumValue("MyEnum.$Member1").Value.Should().Be(0);
            FindEnumValue("MyEnum.$Member2").Value.Should().Be(1);
        }

        [Test]
        public void EnumWithExplicitValuesIsCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetEnumValueName = v => "$" + v.Name };
            Compile(new[] { "enum MyEnum { Member1 = 2, Member2 }" }, namingConvention: namingConvention);
            FindEnumValue("MyEnum.$Member1").Value.Should().Be(2);
            FindEnumValue("MyEnum.$Member2").Value.Should().Be(3);
        }

        [Test]
        public void EnumWithOtherBaseTypeIsCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetEnumValueName = v => "$" + v.Name };
            Compile(new[] { "enum MyEnum : short { Member1, Member2 }" }, namingConvention: namingConvention);
            FindEnum("MyEnum").Values.Should().HaveCount(2);
        }

        [Test]
        public void EnumValueInitialiserCanReferenceOtherMember() {
            var namingConvention = new MockNamingConventionResolver { GetEnumValueName = v => "$" + v.Name };
            Compile(new[] { "enum MyEnum { Member1 = 2, Member2 = Member1 }" }, namingConvention: namingConvention);
            FindEnumValue("MyEnum.$Member1").Value.Should().Be(2);
            FindEnumValue("MyEnum.$Member2").Value.Should().Be(2);
        }

        [Test]
        public void EnumValueInitialiserCanUseComplexExpressions() {
            var namingConvention = new MockNamingConventionResolver { GetEnumValueName = v => "$" + v.Name };
            Compile(new[] { "enum MyEnum { Member1 = 2, Member2 = (int)(2 * (Member1 + (float)3) - 2) }" }, namingConvention: namingConvention);
            FindEnumValue("MyEnum.$Member1").Value.Should().Be(2);
            FindEnumValue("MyEnum.$Member2").Value.Should().Be(8);
        }

        [Test]
        public void EnumValueInitialiserCanReferenceLaterValue() {
            var namingConvention = new MockNamingConventionResolver { GetEnumValueName = v => "$" + v.Name };
            Compile(new[] { "enum MyEnum { Member1 = Member2, Member2 = 4 }" }, namingConvention: namingConvention);
            FindEnumValue("MyEnum.$Member1").Value.Should().Be(4);
            FindEnumValue("MyEnum.$Member2").Value.Should().Be(4);
        }

        [Test]
        public void ParameterNamesAreCorrect() {
            Assert.Inconclusive("TODO, for all kinds of methods");
        }

        [Test]
        public void IndexerThatIsNotUsableFromScriptIsNotImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.NotUsableFromScript() };
            Compile(new[] { "class C { public int this[int i] { get {} set {} } }" }, namingConvention: namingConvention);
            FindClass("C").InstanceMethods.Should().BeEmpty();
            FindClass("C").StaticMethods.Should().BeEmpty();
        }

        [Test]
        public void IndexerWithGetAndSetMethodsIsCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.GetAndSetMethods(MethodImplOptions.InstanceMethod("get_SomeProp"), MethodImplOptions.StaticMethod("set_SomeProp", addThisAsFirstArgument: true)) };
            Compile(new[] { "class C { public int this[int i] { get {} set {} } }" }, namingConvention: namingConvention);
            FindInstanceMethod("C.get_SomeProp").Should().NotBeNull();
            FindStaticMethod("C.set_SomeProp").Should().NotBeNull();
        }

        [Test]
        public void IndexerWithGetAndSetMethodsWithNoCodeIsCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.GetAndSetMethods(MethodImplOptions.InstanceMethod("get_SomeProp", generateCode: false), MethodImplOptions.StaticMethod("set_SomeProp", generateCode: false)) };
            Compile(new[] { "class C { public int this[int i] { get { return 0; } set {} } }" }, namingConvention: namingConvention);
            FindInstanceMethod("C.get_SomeProp").Should().BeNull();
            FindInstanceMethod("C.set_SomeProp").Should().BeNull();
            FindClass("C").StaticMethods.Should().BeEmpty();
        }

        [Test]
        public void NativeIndexerIsCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.NativeIndexer() };
            Compile(new[] { "class C { public int this[int i] { get { return 0; } set {} } }" }, namingConvention: namingConvention);
            FindClass("C").InstanceMethods.Should().BeEmpty();
            FindClass("C").StaticMethods.Should().BeEmpty();
        }

        [Test]
        public void ReadOnlyIndexerWithGetAndSetMethodsIsCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.GetAndSetMethods(MethodImplOptions.InstanceMethod("get_SomeProp"), MethodImplOptions.InstanceMethod("set_SomeProp")) };
            Compile(new[] { "class C { public int this[int i] { get { return 0; } } }" }, namingConvention: namingConvention);
            FindInstanceMethod("C.get_SomeProp").Should().NotBeNull();
            FindInstanceMethod("C.set_SomeProp").Should().BeNull();
            FindClass("C").StaticMethods.Should().BeEmpty();
        }

        [Test]
        public void ReadOnlyNativeIndexerIsCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.NativeIndexer() };
            Compile(new[] { "class C { public int this[int i] { get { return 0; } } }" }, namingConvention: namingConvention);
            FindClass("C").InstanceMethods.Should().BeEmpty();
            FindClass("C").StaticMethods.Should().BeEmpty();
        }

        [Test]
        public void WriteOnlyIndexerWithGetAndSetMethodsIsCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.GetAndSetMethods(MethodImplOptions.InstanceMethod("get_SomeProp"), MethodImplOptions.InstanceMethod("set_SomeProp")) };
            Compile(new[] { "class C { public int this[int i] { set {} } }" }, namingConvention: namingConvention);
            FindInstanceMethod("C.get_SomeProp").Should().BeNull();
            FindInstanceMethod("C.set_SomeProp").Should().NotBeNull();
            FindClass("C").StaticMethods.Should().BeEmpty();
        }

        [Test]
        public void WriteOnlyNativeIndexerIsCorrectlyImported() {
            var namingConvention = new MockNamingConventionResolver { GetPropertyImplementation = p => PropertyImplOptions.NativeIndexer() };
            Compile(new[] { "class C { public int this[int i] { set {} } }" }, namingConvention: namingConvention);
            FindClass("C").InstanceMethods.Should().BeEmpty();
            FindClass("C").StaticMethods.Should().BeEmpty();
        }
    }
}
