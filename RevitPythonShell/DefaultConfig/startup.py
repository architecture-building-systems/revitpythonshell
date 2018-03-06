# script that is run when Revit starts in the IExternalApplication.Startup event.
try:
    # add your code here
    # ...
    __window__.Close()  # closes the window 
except:
    import traceback       # note: add a python27 library to your search path first!
    traceback.print_exc()  # helps you debug when things go wrong