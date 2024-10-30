package com.arcane.arcana.support.controller;

import com.arcane.arcana.support.service.SupportService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;

@RestController
@RequestMapping("/support")
public class SupportController {

    private final SupportService supportService;

    public SupportController(SupportService supportService) {
        this.supportService = supportService;
    }

    @PostMapping(consumes = "multipart/form-data")
    public ResponseEntity<String> handleSupportRequest(
        @RequestParam("category") String category,
        @RequestParam("email") String email,
        @RequestParam("title") String title,
        @RequestParam("description") String description,
        @RequestPart(value = "screenshots", required = false) MultipartFile[] screenshots) {

        try {
            supportService.processSupportRequest(category, email, title, description, screenshots);
            return ResponseEntity.ok("문의가 성공적으로 접수되었습니다.");
        } catch (Exception e) {
            return ResponseEntity.status(500).body("문의 접수 중 오류가 발생했습니다.");
        }
    }
}
