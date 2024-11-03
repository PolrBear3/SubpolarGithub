// copy and pastes every text from <iostream> or file "Calculation.cpp"
#include <iostream>
#include "Calculation.cpp"

// macro
// if David is used, Awesome is the macro
#define David "Awesome"

int main()
{
    // message on console
    std::cout << "hello" << std::endl;

    // waits for input before close
    // std::cin.get();
}

// checks condition whether to save to file and run
#if defined(David)
void activeFunctionDefine()
{
    std::cout << "hello" << std::endl;
}
#endif

#if defined(Sam)
void inActiveFunction()
{
    std::cout << "hello" << std::endl;
}
#endif

#if 1
void activeFunctionNum()
{
    std::cout << "hello" << std::endl;
}
#endif

#if 0
void inActiveFunction()
{
    std::cout << "hello" << std::endl;
}
#endif