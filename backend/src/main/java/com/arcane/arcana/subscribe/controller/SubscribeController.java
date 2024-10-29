package com.arcane.arcana.subscribe.controller;

import com.arcane.arcana.subscribe.dto.SubscribeDto;
import com.arcane.arcana.subscribe.service.SubscribeService;
import com.arcane.arcana.common.dto.ApiResponse;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/subscribe")
public class SubscribeController {

    private final SubscribeService subscribeService;

    public SubscribeController(SubscribeService subscribeService) {
        this.subscribeService = subscribeService;
    }

    @PostMapping
    public ResponseEntity<ApiResponse> subscribe(@RequestBody SubscribeDto subscribeDto) {
        subscribeService.subscribe(subscribeDto.getSubscribe());
        return ResponseEntity.ok(new ApiResponse("구독 성공", null));
    }
}
