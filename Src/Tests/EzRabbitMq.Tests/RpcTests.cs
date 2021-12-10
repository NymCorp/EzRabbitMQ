using System;
using System.Threading.Tasks;
using EzRabbitMQ.Tests.Messages;
using Xunit;
using Xunit.Abstractions;

namespace EzRabbitMQ.Tests
{
    public class RpcTests
    {
        private readonly ITestOutputHelper _output;

        public RpcTests(ITestOutputHelper output) => _output = output;

        [Fact]
        public async Task CanDoRpcClientCall()
        {
            var (mailbox, _, _) = TestUtils.Build<RpcTests>(_output);

            using var server = mailbox.RpcServer<RpcServerTest>();

            using var client = mailbox.RpcClient();

            var message = Guid.NewGuid().ToString();

            var response = await client.CallAsync<RpcSampleResponse>(new RpcSampleRequest {Text = message}, default);

            Assert.Equal(message, response?.Model.Text);
        }

        [Fact]
        public void CanDoMultipleRpcClientCall()
        {
            var (mailbox, _, _) = TestUtils.Build<RpcTests>(_output);

            using var server = mailbox.RpcServer<RpcServerTest>();

            using var client = mailbox.RpcClient();

            var message = Guid.NewGuid().ToString();

            var response = client.Call<RpcSampleResponse>(new RpcSampleRequest {Text = message});

            Assert.NotNull(response);

            Assert.Equal(message, response.Model.Text);

            using var client2 = mailbox.RpcClient();

            var message2 = Guid.NewGuid().ToString();

            var response2 = client.Call<RpcSampleResponse>(new RpcSampleRequest {Text = message2});

            Assert.Equal(message2, response2?.Model.Text);
        }

        [Fact]
        public void CanDoMultipleRpcClientCalls()
        {
            var (mailbox, _, _) = TestUtils.Build<RpcTests>(_output);

            using var server = mailbox.RpcServer<RpcServerTest>();

            using var client = mailbox.RpcClient();

            var message = Guid.NewGuid().ToString();

            var response = client.Call<RpcSampleResponse>(new RpcSampleRequest {Text = message});

            Assert.NotNull(response);

            Assert.Equal(message, response.Model.Text);

            var message2 = Guid.NewGuid().ToString();

            var response2 = client.Call<RpcSampleResponse>(new RpcSampleRequest {Text = message2});

            Assert.NotNull(response2);

            Assert.Equal(message2, response2.Model.Text);
        }

        [Fact]
        public void CanDoTimeout()
        {
            var (mailbox, _, _) = TestUtils.Build<RpcTests>(_output, true);

            using var client = mailbox.RpcClient();

            var message = Guid.NewGuid().ToString();

            var response = client.Call<RpcSampleResponse>(new RpcSampleRequest {Text = message});

            Assert.Null(response);
        }

        [Fact]
        public void CanUseMultipleRpcServer()
        {
            var (mailbox, _, _) = TestUtils.Build<RpcTests>(_output, true);

            using var serverA = mailbox.RpcServer<RpcServerTest>();
            using var serverB = mailbox.RpcServer<RpcServerTest>("serverB");

            using var client = mailbox.RpcClient();
            using var clientB = mailbox.RpcClient("serverB");

            var messageA = Guid.NewGuid().ToString();
            var messageB = Guid.NewGuid().ToString();

            var responseB = clientB.Call<RpcSampleResponse>(new RpcSampleRequest {Text = messageB});
            var response = client.Call<RpcSampleResponse>(new RpcSampleRequest {Text = messageA});

            Assert.Equal(messageA, response?.Model.Text);
            Assert.Equal(messageB, responseB?.Model.Text);
        }
        
        [Fact]
        public void CanUseMultipleRpcClient()
        {
            var (mailbox, _, _) = TestUtils.Build<RpcTests>(_output);

            using var serverA = mailbox.RpcServer<RpcServerTest>();

            using var client = mailbox.RpcClient();
            using var clientB = mailbox.RpcClient();

            var messageA = Guid.NewGuid().ToString();
            var messageB = Guid.NewGuid().ToString();

            var responseB = clientB.Call<RpcSampleResponse>(new RpcSampleRequest {Text = messageB});
            var response = client.Call<RpcSampleResponse>(new RpcSampleRequest {Text = messageA});

            Assert.Equal(messageA, response?.Model.Text);
            Assert.Equal(messageB, responseB?.Model.Text);
        }

        [Fact]
        public void CanReceiveMultipleHandleInOneServer()
        {
            var (mailbox, _, _) = TestUtils.Build<RpcTests>(_output);

            using var client = mailbox.RpcClient();
            using var serverA = mailbox.RpcServer<RpcServerTest>();

            var message = Guid.NewGuid().ToString();
            var response = client.Call<RpcIncrementResponse>(new RpcIncrementRequest {CurrentValue = 1});
            var response2 = client.Call<RpcSampleResponse>(new RpcSampleRequest {Text = message});

            Assert.Equal(2, response?.NewValue);
            Assert.Equal(message, response2?.Model.Text);
        }


        [Fact]
        public void CanSendMultipleMessage()
        {
            var (mailbox, _, _) = TestUtils.Build<RpcTests>(_output);

            using var client = mailbox.RpcClient();
            using var serverA = mailbox.RpcServer<RpcServerTest>();

            for (var i = 0; i < 100; i++)
            {
                var response = client.Call<RpcIncrementResponse>(new RpcIncrementRequest {CurrentValue = i});
                Assert.Equal(i + 1, response?.NewValue);
            }
        }

        [Fact]
        public void CanSendMessageThatIsNotExcepted()
        {
            var (mailbox, _, _) = TestUtils.Build<RpcTests>(_output);
            
            using var client = mailbox.RpcClient();
            using var serverA = mailbox.RpcServer<RpcServerTest>();
            
            var response = client.Call<RpcUnexpectedResponse>(new RpcUnexpectedRequest());
            
            Assert.Null(response);
        }
        
        [Fact]
        public void CanRecoverFromUnexpectedMessage()
        {
            var (mailbox, _, _) = TestUtils.Build<RpcTests>(_output);
            
            using var serverA = mailbox.RpcServer<RpcServerTest>();
            using var client = mailbox.RpcClient();
            
            var unexpectedResponse = client.Call<RpcUnexpectedResponse>(new RpcUnexpectedRequest());
            var response = client.Call<RpcIncrementResponse>(new RpcIncrementRequest {CurrentValue = 0});

            Assert.Equal(1, response?.NewValue);
            Assert.Null(unexpectedResponse);
        }
    }
}