namespace CdnGetter;

public sealed class LazyTransform<T, TOut>
{
    private readonly object _syncRoot = new();
    private Func<TOut> _getOutput;
    private Func<T> _getInput;

    public T Input { get; private set; }

    public T GetInput()
    {
        lock (_syncRoot)
            return _getInput();
    }

    public TOut GetOutput()
    {
        lock (_syncRoot)
            return _getOutput();
    }

    public LazyTransform(T input, Func<T, TOut> transformer)
    {
        Input = input;
        _getInput = () =>
        {
            _getInput = () => Input;
            try
            {
                TOut output = transformer(input);
                _getOutput = () => output;
            }
            catch (Exception exception)
            {
                _getOutput = () => throw exception;
            }
            return Input;
        };
        _getOutput = () =>
        {
            _getInput = () => Input;
            try
            {
                TOut output = transformer(input);
                _getOutput = () => output;
                return output;
            }
            catch (Exception exception)
            {
                _getOutput = () => throw exception;
                throw;
            }
        };
    }

    public LazyTransform(T input, Func<T, Action<T>, TOut> transformer)
    {
        Input = input;
        _getInput = () =>
        {
            _getInput = () => Input;
            try
            {
                TOut output = transformer(input, v => Input = v);
                _getOutput = () => output;
            }
            catch (Exception exception)
            {
                _getOutput = () => throw exception;
            }
            return Input;
        };
        _getOutput = () =>
        {
            _getInput = () => Input;
            try
            {
                TOut output = transformer(input, v => Input = v);
                _getOutput = () => output;
                return output;
            }
            catch (Exception exception)
            {
                _getOutput = () => throw exception;
                throw;
            }
        };
    }
}

public sealed class LazyTransform<T1, T2, TOut>
{
    private readonly object _syncRoot = new();
    private Func<TOut> _getOutput;
    private Func<T1> _getInput1;
    private Func<T2> _getInput2;

    public T1 Input1 { get; private set; }

    public T2 Input2 { get; private set; }

    public T1 GetInput1()
    {
        lock (_syncRoot)
            return _getInput1();
    }

    public T2 GetInput2()
    {
        lock (_syncRoot)
            return _getInput2();
    }

    public TOut GetOutput()
    {
        lock (_syncRoot)
            return _getOutput();
    }

    public LazyTransform(T1 input1, T2 input2, Func<T1, T2, TOut> transformer)
    {
        Input1 = input1;
        Input2 = input2;
        _getInput1 = () =>
        {
            _getInput1 = () => Input1;
            _getInput2 = () => Input2;
            try
            {
                TOut output = transformer(input1, input2);
                _getOutput = () => output;
            }
            catch (Exception exception)
            {
                _getOutput = () => throw exception;
            }
            return Input1;
        };
        _getInput2 = () =>
        {
            _getInput1 = () => Input1;
            _getInput2 = () => Input2;
            try
            {
                TOut output = transformer(input1, input2);
                _getOutput = () => output;
            }
            catch (Exception exception)
            {
                _getOutput = () => throw exception;
            }
            return Input2;
        };
        _getOutput = () =>
        {
            _getInput1 = () => Input1;
            _getInput2 = () => Input2;
            try
            {
                TOut output = transformer(input1, input2);
                _getOutput = () => output;
                return output;
            }
            catch (Exception exception)
            {
                _getOutput = () => throw exception;
                throw;
            }
        };
    }

    public LazyTransform(T1 input1, T2 input2, Func<T1, T2, Action<T1>, TOut> transformer)
    {
        Input1 = input1;
        Input2 = input2;
        _getInput1 = () =>
        {
            _getInput1 = () => Input1;
            _getInput2 = () => Input2;
            try
            {
                TOut output = transformer(input1, input2, v => Input1 = v);
                _getOutput = () => output;
            }
            catch (Exception exception)
            {
                _getOutput = () => throw exception;
            }
            return Input1;
        };
        _getInput2 = () =>
        {
            _getInput1 = () => Input1;
            _getInput2 = () => Input2;
            try
            {
                TOut output = transformer(input1, input2, v => Input1 = v);
                _getOutput = () => output;
            }
            catch (Exception exception)
            {
                _getOutput = () => throw exception;
            }
            return Input2;
        };
        _getOutput = () =>
        {
            _getInput1 = () => Input1;
            _getInput2 = () => Input2;
            try
            {
                TOut output = transformer(input1, input2, v => Input1 = v);
                _getOutput = () => output;
                return output;
            }
            catch (Exception exception)
            {
                _getOutput = () => throw exception;
                throw;
            }
        };
    }

    public LazyTransform(T1 input1, T2 input2, Func<T1, T2, Action<T1, T2>, TOut> transformer)
    {
        Input1 = input1;
        Input2 = input2;
        _getInput1 = () =>
        {
            _getInput1 = () => Input1;
            _getInput2 = () => Input2;
            try
            {
                TOut output = transformer(input1, input2, (v1, v2) => (Input1, Input2) = (v1, v2));
                _getOutput = () => output;
            }
            catch (Exception exception)
            {
                _getOutput = () => throw exception;
            }
            return Input1;
        };
        _getInput2 = () =>
        {
            _getInput1 = () => Input1;
            _getInput2 = () => Input2;
            try
            {
                TOut output = transformer(input1, input2, (v1, v2) => (Input1, Input2) = (v1, v2));
                _getOutput = () => output;
            }
            catch (Exception exception)
            {
                _getOutput = () => throw exception;
            }
            return Input2;
        };
        _getOutput = () =>
        {
            _getInput1 = () => Input1;
            _getInput2 = () => Input2;
            try
            {
                TOut output = transformer(input1, input2, (v1, v2) => (Input1, Input2) = (v1, v2));
                _getOutput = () => output;
                return output;
            }
            catch (Exception exception)
            {
                _getOutput = () => throw exception;
                throw;
            }
        };
    }
}
