//
//  UnityIAP.m
//  Unity-iPhone
//
//  Created by liyong on 15/10/7.
//
//

#import "SimpleIAP.h"
#include "string.h"

    
    void requestProducts(const char* productIdentifiers){
        NSError *error = NULL;
        
        id data = [NSData dataWithBytes:productIdentifiers length:strlen(productIdentifiers)];
        id pro = [NSJSONSerialization JSONObjectWithData:data options:NSJSONReadingMutableContainers error:&error];
        [[IAPHelper sharedHelper] requestProducts: pro];
        
    }

void Charge(const char* chargeItem) {
    //NSError *error = NULL;
    //id pro = [NSJSONSerialization JSONObjectWithData:chargeItem options:NSJSONReadingMutableContainers error:&error];
    id str = [NSString stringWithUTF8String:chargeItem];
    [[IAPHelper sharedHelper] buyProductIdentifier:str];
}
