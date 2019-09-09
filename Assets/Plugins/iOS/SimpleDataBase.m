//
//  SimpleDataBase.m
//  Unity-iPhone
//
//  Created by liyong on 15/10/7.
//
//

#import "SimpleDataBase.h"


@implementation SimpleDataBase
@synthesize dic = _dic;

static SimpleDataBase * _sharedInstance;

+ (SimpleDataBase *) sharedInstance {
    
    if (_sharedInstance != nil) {
        return _sharedInstance;
    }
    _sharedInstance = [[SimpleDataBase alloc] init];
    _sharedInstance.dic = [NSMutableDictionary dictionary];
    return _sharedInstance;
}



@end
