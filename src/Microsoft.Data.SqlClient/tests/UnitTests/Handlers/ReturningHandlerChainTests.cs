// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClientX.Handlers;
using Moq;
using Xunit;

namespace Microsoft.Data.SqlClient.NetCore.UnitTests.Handlers
{
    public class ReturningHandlerChainTests
    {
        [Fact]
        public async Task Handle_ThrowCollected_AllPass_ThrowsNoHandlerFound()
        {
            // Arrange
            var handlers = new[] { GetPassingHandler(), GetPassingHandler(), GetPassingHandler() };
            var chain = new ReturningHandlerChain<string, string>(
                GetHandlers(handlers),
                ReturningHandlerChainExceptionBehavior.ThrowCollected);

            // Act
            Func<Task> action = () => chain.Handle("foo", false, default).AsTask();

            // Assert
            await Assert.ThrowsAsync<NoSuitableHandlerFoundException>(action);
            foreach (var mockHandler in handlers)
            {
                mockHandler.Verify(h => h.Handle("foo", false, default), Times.Once);
            }
        }



        private IReturningHandler<string, string>[] GetHandlers(IEnumerable<Mock<IReturningHandler<string, string>>> mockHandlers) =>
            mockHandlers.Select(h => h.Object).ToArray();

        private Mock<IReturningHandler<string, string>> GetPassingHandler()
        {
            var mockHandler = new Mock<IReturningHandler<string, string>>();
            mockHandler.Setup(m => m.Handle(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()));

            return mockHandler;
        }

        // private Mock<IReturningHandler<string, string>> GetSuccessHandler()
        // {
        //
        // }
        //
        // private Mock<IReturningHandler<string, string>> GetThrowingHandler()
        // {
        //
        // }
    }
}
