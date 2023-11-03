namespace CdnGetter;

public sealed class LazyAsyncTransform<T, TOut>
{
    private readonly object _syncRoot = new();
    private Func<Task<T>> _getInputAsync;
    private Func<Task<TOut>> _getOutputAsync;

    public T Input { get; private set; }

    public Task<T> GetInputAsync()
    {
        lock (_syncRoot)
            return _getInputAsync();
    }

    public Task<TOut> GetOutputAsync()
    {
        lock (_syncRoot)
            return _getOutputAsync();
    }

    public LazyAsyncTransform(T input, Func<T, Task<TOut>> asyncTransformer)
    {
        Input = input;
        _getInputAsync = async () =>
        {
            _getInputAsync = () => Task.FromResult(input);
            try
            {
                TOut output = await asyncTransformer(input);
                _getOutputAsync = () => Task.FromResult(output);
            }
            catch (Exception exception)
            {
                _getOutputAsync = () => Task.FromException<TOut>(exception);
            }
            return Input;
        };
        _getOutputAsync = async () =>
        {
            _getInputAsync = () => Task.FromResult(input);
            try
            {
                TOut output = await asyncTransformer(input);
                _getOutputAsync = () => Task.FromResult(output);
                return output;
            }
            catch (Exception exception)
            {
                _getOutputAsync = () => Task.FromException<TOut>(exception);
                throw;
            }
        };
    }

    public LazyAsyncTransform(T input, Func<T, Action<T>, Task<TOut>> asyncTransformer)
    {
        Input = input;
        _getInputAsync = async () =>
        {
            try
            {
                TOut output = await asyncTransformer(input, v => Input = v);
                _getOutputAsync = () => Task.FromResult(output);
            }
            catch (Exception exception)
            {
                _getOutputAsync = () => Task.FromException<TOut>(exception);
            }
            _getInputAsync = () => Task.FromResult(Input);
            return Input;
        };
        _getOutputAsync = async () =>
        {
            _getInputAsync = () => Task.FromResult(input);
            try
            {
                TOut output = await asyncTransformer(input, v => Input = v);
                _getOutputAsync = () => Task.FromResult(output);
                _getInputAsync = () => Task.FromResult(Input);
                return output;
            }
            catch (Exception exception)
            {
                _getOutputAsync = () => Task.FromException<TOut>(exception);
                _getInputAsync = () => Task.FromResult(Input);
                throw;
            }
        };
    }
}

public sealed class LazyAsyncTransform<T1, T2, TOut>
{
    private readonly object _syncRoot = new();
    private Func<Task<T1>> _getInput1Async;
    private Func<Task<T2>> _getInput2Async;
    private Func<Task<TOut>> _getOutputAsync;

    public T1 Input1 { get; private set; }

    public T2 Input2 { get; private set; }

    public Task<T1> GetInput1Async()
    {
        lock (_syncRoot)
            return _getInput1Async();
    }

    public Task<T2> GetInput2Async()
    {
        lock (_syncRoot)
            return _getInput2Async();
    }

    public Task<TOut> GetOutputAsync()
    {
        lock (_syncRoot)
            return _getOutputAsync();
    }

    public LazyAsyncTransform(T1 input1, T2 input2, Func<T1, T2, Task<TOut>> asyncTransformer)
    {
        Input1 = input1;
        Input2 = input2;
        _getInput1Async = async () =>
        {
            _getInput1Async = () => Task.FromResult(input1);
            _getInput2Async = () => Task.FromResult(input2);
            try
            {
                TOut output = await asyncTransformer(input1, input2);
                _getOutputAsync = () => Task.FromResult(output);
            }
            catch (Exception exception)
            {
                _getOutputAsync = () => Task.FromException<TOut>(exception);
            }
            return input1;
        };
        _getInput2Async = async () =>
        {
            _getInput1Async = () => Task.FromResult(input1);
            _getInput2Async = () => Task.FromResult(input2);
            try
            {
                TOut output = await asyncTransformer(input1, input2);
                _getOutputAsync = () => Task.FromResult(output);
            }
            catch (Exception exception)
            {
                _getOutputAsync = () => Task.FromException<TOut>(exception);
            }
            return input2;
        };
        _getOutputAsync = async () =>
        {
            _getInput1Async = () => Task.FromResult(input1);
            _getInput2Async = () => Task.FromResult(input2);
            try
            {
                TOut output = await asyncTransformer(input1, input2);
                _getOutputAsync = () => Task.FromResult(output);
                return output;
            }
            catch (Exception exception)
            {
                _getOutputAsync = () => Task.FromException<TOut>(exception);
                throw;
            }
        };
    }

    public LazyAsyncTransform(T1 input1, T2 input2, Func<T1, T2, Action<T1>, Task<TOut>> asyncTransformer)
    {
        Input1 = input1;
        Input2 = input2;
        _getInput1Async = async () =>
        {
            try
            {
                TOut output = await asyncTransformer(input1, input2, v => Input1 = v);
                _getOutputAsync = () => Task.FromResult(output);
            }
            catch (Exception exception)
            {
                _getOutputAsync = () => Task.FromException<TOut>(exception);
            }
            return input1;
        };
        _getInput2Async = async () =>
        {
            try
            {
                TOut output = await asyncTransformer(input1, input2, v => Input1 = v);
                _getOutputAsync = () => Task.FromResult(output);
            }
            catch (Exception exception)
            {
                _getOutputAsync = () => Task.FromException<TOut>(exception);
            }
            _getInput1Async = () => Task.FromResult(Input1);
            _getInput2Async = () => Task.FromResult(Input2);
            return input2;
        };
        _getOutputAsync = async () =>
        {
            try
            {
                TOut output = await asyncTransformer(input1, input2, v => Input1 = v);
                _getOutputAsync = () => Task.FromResult(output);
                _getInput1Async = () => Task.FromResult(Input1);
                _getInput2Async = () => Task.FromResult(Input2);
                return output;
            }
            catch (Exception exception)
            {
                _getOutputAsync = () => Task.FromException<TOut>(exception);
                _getInput1Async = () => Task.FromResult(Input1);
                _getInput2Async = () => Task.FromResult(Input2);
                throw;
            }
        };
    }

    public LazyAsyncTransform(T1 input1, T2 input2, Func<T1, T2, Action<T1, T2>, Task<TOut>> asyncTransformer)
    {
        Input1 = input1;
        Input2 = input2;
        _getInput1Async = async () =>
        {
            try
            {
                TOut output = await asyncTransformer(input1, input2, (v1, v2) => (Input1, Input2) = (v1, v2));
                _getOutputAsync = () => Task.FromResult(output);
            }
            catch (Exception exception)
            {
                _getOutputAsync = () => Task.FromException<TOut>(exception);
            }
            return input1;
        };
        _getInput2Async = async () =>
        {
            try
            {
                TOut output = await asyncTransformer(input1, input2, (v1, v2) => (Input1, Input2) = (v1, v2));
                _getOutputAsync = () => Task.FromResult(output);
            }
            catch (Exception exception)
            {
                _getOutputAsync = () => Task.FromException<TOut>(exception);
            }
            _getInput1Async = () => Task.FromResult(Input1);
            _getInput2Async = () => Task.FromResult(Input2);
            return input2;
        };
        _getOutputAsync = async () =>
        {
            try
            {
                TOut output = await asyncTransformer(input1, input2, (v1, v2) => (Input1, Input2) = (v1, v2));
                _getOutputAsync = () => Task.FromResult(output);
                _getInput1Async = () => Task.FromResult(Input1);
                _getInput2Async = () => Task.FromResult(Input2);
                return output;
            }
            catch (Exception exception)
            {
                _getOutputAsync = () => Task.FromException<TOut>(exception);
                _getInput1Async = () => Task.FromResult(Input1);
                _getInput2Async = () => Task.FromResult(Input2);
                throw;
            }
        };
    }
}
