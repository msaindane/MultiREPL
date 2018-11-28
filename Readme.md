# MultiREPL

This project was a PoC written in 2011 which later on became the [Scripting Shell](https://github.com/Lavakumar/IronWASP/blob/master/IronWASP/IronScripting.cs) in [IronWASP](https://github.com/Lavakumar/IronWASP/). It uses the Microsoft [DLR Hosting API](https://github.com/IronLanguages/dlr) to run Python and Ruby code on .Net

In this PoC, I was experimenting with calling Python functions from Ruby code and vice versa. This sounds crazy, but has some good usecases. E.g. Since most security libraries and tools are written in Python (and I'm a Ruby fan!), I wanted IronWASP to have the ability to reuse code written once in either Python or Ruby and allow it to be extended from either languages.

IronWASP allows you to write a plugin in Ruby for example and then use it from within another plugin that is written in Python and viceversa.

## Compiling

You can compile the code using:

```shell
msbuild MultiREPL.sln
```

## Sample Execution

After compiling, run the exe from the MultiREPL/bin/Release folder
```shell
# cd MultiREPL/bin/Release
# MultiREPL.exe
# Type #help to know more.
rb> #help
  Available commands:
	#ls	Lists all Languages supported.
	#rb	Change language to Ruby.
	#py	Change language to Python.
	#exit	Exits the application.
	#help	This cruft
rb> #ls
IronRuby
IronPython 2.6.1
rb> class Dog
rb| 	def says
rb| 		puts "Woof Woof!"
rb| 	end
rb| end
=> nil
rb> Dog.new.says
Woof Woof!
=> nil
rb> #py
py> import Dog
py> Dog().says()
Woof Woof!
py> class Cat(object):
py| 	def says(self):
py| 		print "Meow Meow!"
py|
py> Cat().says()
Meow Meow!
py> #rb
rb> Cat().new.says
Meow Meow!
=> nil
rb> #exit
```

You can see that a class _"Dog"_ defined in Ruby can be instantiated and methods on that object can be called from Python. Similarly, a class _"Cat"_ defined in Python can be instantiated and called from Ruby.

References:

- <http://matousek.wordpress.com/2009/04/15/python-says-hello-to-ruby/>
- <http://www.voidspace.org.uk/ironpython/hosting_api.shtml>

