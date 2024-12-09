#include "foo.h"
#include "bar.h"
#include "baz.h"
#include "qux.h"

static const char test_var __attribute__((used)) __attribute__((section(".magic"))) = 0xA7;

int main(void)
{
	volatile char c = test_var;
	return 0;
}